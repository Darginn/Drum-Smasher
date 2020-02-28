﻿using System;
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
using DrumSmasher.Settings;
using DrumSmasher.Notes;

namespace DrumSmasher
{
    public class TitleScreenManager : MonoBehaviour
    {
        public Charts.Chart LoadedChart;
        private bool _sceneActionActive;
        public GameObject PlayAlert;
        public GameObject FailAlert;
        public GameObject SettingsMenu;
        public GameObject DevConsolePrefab;
        public GameObject Canvas;
        public Toggle FullscreenToggle;
        
        public Dropdown ResolutionDropdown;

        Resolution[] resolutions;
        private ExtensionFilter _extChartFilter = new ExtensionFilter("Chart", "chart");
        private ExtensionFilter _extOsuFilter = new ExtensionFilter("Osu difficulty", "osu");
        private GameObject _devConsole;
        
        void Start()
        {
            TitleScreenSettings tss = null;

            if (!SettingsManager.SettingsStorage.ContainsKey("TitleScreen"))
            {
                tss = new TitleScreenSettings();
                SettingsManager.AddOrUpdate(tss);

                tss.Load();
            }
            else
                tss = SettingsManager.SettingsStorage["TitleScreen"] as TitleScreenSettings;


            TaikoSettings ts = null;
            if (!SettingsManager.SettingsStorage.ContainsKey("Taiko"))
            {
                ts = new TaikoSettings();
                SettingsManager.AddOrUpdate(ts);

                ts.Load();
            }
            else
                ts = SettingsManager.SettingsStorage["TitleScreen"] as TaikoSettings;

            //Initialize static AutoInit Attributes
            System.Reflection.Assembly.GetExecutingAssembly().ActivateAttributeMethods<AutoInitAttribute>();

            resolutions = Screen.resolutions;

            ResolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = $"{resolutions[i].width} x {resolutions[i].height} {resolutions[i].refreshRate} hz";
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height &&
                    resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                    currentResolutionIndex = i;
            }

            ResolutionDropdown.AddOptions(options);
            ResolutionDropdown.value = currentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();

            SetFullscreen(tss.Data.Fullscreen);
            SetResolution(tss.Data.ScreenWidth, tss.Data.ScreenHeight, tss.Data.RefreshRate);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                if (_devConsole != null)
                {
                    if (_devConsole.activeSelf)
                        _devConsole.SetActive(false);
                    else
                        _devConsole.SetActive(true);
                }
                else
                {
                    _devConsole = Instantiate(DevConsolePrefab);
                    _devConsole.transform.SetParent(Canvas.transform);

                    RectTransform rt = _devConsole.GetComponent<RectTransform>();        
                    rt.anchoredPosition3D = new Vector3(950, 347, 1.5f);

                    StartCoroutine(rt.MoveOverSeconds(rt.anchoredPosition3D, new Vector3(950, -383, 1.5f), 0.5f, true));
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position -= new Vector3(5, 0, 0);
                Logger.Log($"Moved from X {old.x} to {_devConsole.transform.position.x}, local: {_devConsole.transform.localPosition.x}, oldLocal: {oldLocal.x}");
            }

            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position += new Vector3(5, 0, 0);
                Logger.Log($"Moved from X {old.x} to {_devConsole.transform.position.x}, local: {_devConsole.transform.localPosition.x}, oldLocal: {oldLocal.x}");
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position += new Vector3(0, 5, 0);
                Logger.Log($"Moved from Y {old.y} to {_devConsole.transform.position.y}, local: {_devConsole.transform.localPosition.y}, oldLocal: {oldLocal.y}");
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position -= new Vector3(0, 5, 0);
                Logger.Log($"Moved from Y {old.y} to {_devConsole.transform.position.y}, local: {_devConsole.transform.localPosition.y}, oldLocal: {oldLocal.y}");
            }
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
            Logger.Log("Selecting .osu file");
            
            string path = StandaloneFileBrowser.OpenFilePanel("Select .osu file", Application.dataPath + "/../Charts/", new ExtensionFilter[] { _extOsuFilter }, false)[0];
            
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

            TitleScreenSettings tss = SettingsManager.SettingsStorage["TitleScreen"] as TitleScreenSettings;

            DirectoryInfo chartPath = new DirectoryInfo(tss.Data.ChartPath + $"/{artist} - {title} ({creator})/");
            
            Charts.ChartFile.Save(chart, chartPath);
            
            FileInfo audio = new FileInfo(Path.Combine(chartPath.FullName, chart.SoundFile));

            if(!audio.Exists)
                File.Copy(chartFile.Directory.FullName + @"\" + chart.SoundFile, audio.FullName);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            TitleScreenSettings tss = SettingsManager.SettingsStorage["TitleScreen"] as TitleScreenSettings;
            tss.Data.Fullscreen = isFullscreen;

            Screen.fullScreen = tss.Data.Fullscreen;
            FullscreenToggle.isOn = tss.Data.Fullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            SetResolution(resolution.width, resolution.height, resolution.refreshRate);
        }

        public void SetResolution(int width, int height, int refreshRate)
        {
            TitleScreenSettings tss = SettingsManager.SettingsStorage["TitleScreen"] as TitleScreenSettings;

            tss.Data.ScreenWidth = width;
            tss.Data.ScreenHeight = height;
            tss.Data.RefreshRate = refreshRate;
            
            Screen.SetResolution(width, height, tss.Data.Fullscreen);
            string optionStr = $"{width} x {height} {refreshRate} hz";
            int option = ResolutionDropdown.options.FindIndex(o => o.text.Equals(optionStr));

            if (option == -1)
            {
                Logger.Log("Could not find option: " + optionStr, LogLevel.ERROR);
                return;
            }

            var op = ResolutionDropdown.options.ElementAt(option);
            ResolutionDropdown.options.RemoveAt(option);
            ResolutionDropdown.options.Insert(0, op);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            SettingsManager.SaveSettings();
#else
            Application.Quit();
#endif
            Logger.Dispose();
        }

        void OnApplicationExit()
        {
            SettingsManager.SaveSettings();
            Logger.Dispose();
        }

    }
}
