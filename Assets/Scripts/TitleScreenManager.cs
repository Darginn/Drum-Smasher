using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

        void Start()
        {
            resolutions = Screen.resolutions;

            ResolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }

            ResolutionDropdown.AddOptions(options);
            ResolutionDropdown.value = currentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();
        }

        void Update()
        {

        }

        public void SwitchToTaiko()
        {
            if (_sceneActionActive)
                return;
            else if (LoadedChart == null)
            {
                PlayAlert.SetActive(true);
                return;
            }

            _sceneActionActive = true;

            Logger.Log("LoadedChart: " + (LoadedChart == null));

            AsyncOperation loadAO = SceneManager.LoadSceneAsync("Main");
            loadAO.completed += ao =>
            {
                _sceneActionActive = false;
                GameManager.OnSceneLoaded(LoadedChart);
            };
        }

        public void LoadMap()
        {
            if (_sceneActionActive)
                return;

            _sceneActionActive = true;

            Logger.Log("Selecting chart");
            string path = EditorUtility.OpenFilePanel("Select Chart", Application.dataPath, "chart");

            if (path.Length < 0)
                return;

            Logger.Log("Loading path");
            LoadedChart = Charts.ChartFile.Load(path);

            if (LoadedChart == null)
            {
                FailAlert.SetActive(true);
                Logger.Log("Chart failed to load");
                return;
            }

            if (LoadedChart != null)
                Logger.Log("Chart loaded");
            else
                Logger.Log("Failed to load chart");

            _sceneActionActive = false;
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
            string path = EditorUtility.OpenFilePanel("Select osu map", Application.dataPath, "osu");
            string output = EditorUtility.SaveFilePanel("Save chart to", Application.dataPath, path.Remove(0, path.LastIndexOf('/') + 1).Replace(".osu", ""), "chart");

            if (path.Length <= 0 || output.Length <= 0)
                return;
            
            var chart = Charts.ChartFile.ConvertOsuFile(path);

            Charts.ChartFile.Save(output, chart);
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
