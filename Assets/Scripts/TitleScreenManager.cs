using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;
using System.IO;
using System.Collections;

namespace DrumSmasher
{
    public class TitleScreenManager : MonoBehaviour
    {
        public Charts.Chart LoadedChart;
        private bool _sceneActionActive;
        public GameObject PlayAlert;
        public GameObject FailAlert;
        public GameObject SettingsMenu;

        public Dropdown ResolutionDropdown;

        Resolution[] resolutions;
        private ExtensionFilter _extChartFilter = new ExtensionFilter("Chart", "chart");
        private ExtensionFilter _extOsuFilter = new ExtensionFilter("Osu difficulty", "osu");

        void Start()
        {
            resolutions = Screen.resolutions;

            ResolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "hz";
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height &&
                    resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                    currentResolutionIndex = i;
            }

            ResolutionDropdown.AddOptions(options);
            ResolutionDropdown.value = currentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();
        }

        void Update()
        {

        }

        public void SwitchToSonglist()
        {
            SceneManager.LoadScene("Song List");
        }
        
        public void Settings()
        {
            if (SettingsMenu.activeSelf == false)
                SettingsMenu.SetActive(true);
            else
                SettingsMenu.SetActive(false);
        }
        
        public void ConvertOsuMap()
        {
            Logger.Log("Selecting osu map");
            
            string path = StandaloneFileBrowser.OpenFilePanel("Select osu map", Application.dataPath + "/../", new ExtensionFilter[] { _extOsuFilter }, false)[0];
            
            if (path.Length <= 0)
                return;

            FileInfo chartFile = new FileInfo(path);

            if (!chartFile.Exists)
            {
                if (new DirectoryInfo(path).Exists)
                {
                    Logger.Log("Converting folders is not supported yet");
                    return;
                }

                Logger.Log("Chartfile not found");
                return;
            }
            
            var chart = Charts.ChartFile.ConvertOsuFile(path);
            string artist = Charts.ChartFile.FixPath(chart.Artist);
            string title = Charts.ChartFile.FixPath(chart.Title);
            string creator = Charts.ChartFile.FixPath(chart.Creator);

            DirectoryInfo chartPath = new DirectoryInfo(Application.dataPath + $"/../Charts/{artist} - {title} ({creator})/");
            
            Charts.ChartFile.Save(chart, chartPath);
            
            FileInfo audio = new FileInfo(Path.Combine(chartPath.FullName, chart.SoundFile));

            if(!audio.Exists)
                File.Copy(chartFile.Directory.FullName + @"\" + chart.SoundFile, audio.FullName);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void OnApplicationExit()
        {
            Logger.Dispose();
        }

    }
}
