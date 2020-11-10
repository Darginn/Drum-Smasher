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
using DrumSmasher.Assets.Scripts.Settings;
using System.Threading;
using DSServerCommon;

namespace DrumSmasher.Assets.Scripts
{
    public class TitleScreenManager : MonoBehaviour
    {
        public Charts.Chart LoadedChart;
        bool _sceneActionActive;
        public GameObject PlayAlert;
        public GameObject FailAlert;
        public GameObject SettingsMenu;
        public GameObject DevConsolePrefab;
        public GameObject Canvas;
        public Toggle FullscreenToggle;
        
        public Dropdown ResolutionDropdown;

        Resolution[] resolutions;
        ExtensionFilter _extChartFilter = new ExtensionFilter("Chart", "chart");
        ExtensionFilter _extOsuFilter = new ExtensionFilter("Osu difficulty", "osu");
        GameObject _devConsole;

        [SerializeField] Text key1Text;
        [SerializeField] Text key2Text;
        [SerializeField] Text key3Text;
        [SerializeField] Text key4Text;
        
        public void SetHotKey1()
        {
            StartCoroutine(SetHotKeyCoroutine(0));
        }

        public void SetHotKey2()
        {
            StartCoroutine(SetHotKeyCoroutine(1));
        }

        public void SetHotKey3()
        {
            StartCoroutine(SetHotKeyCoroutine(2));
        }

        public void SetHotKey4()
        {
            StartCoroutine(SetHotKeyCoroutine(3));
        }

        IEnumerator SetHotKeyCoroutine(int value)
        {
            KeyCode? key = null;

            while(!key.HasValue)
            {
                if (Input.GetKey(KeyCode.Escape))
                    break;

                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(keyCode))
                        key = keyCode;

                yield return new WaitForEndOfFrame();
            }

            if (!key.HasValue)
                yield return null;

            SetHotKey(value, key.Value);
        }

        /// <param name="value">0-3</param>
        void SetHotKey(int value, KeyCode key)
        {
            TaikoSettings ts = SettingsManager.SettingsStorage["Taiko"] as TaikoSettings;

            switch(value)
            {
                default:
                case 0:
                    key1Text.text = key.ToString();
                    ts.Data.Key1 = key1Text.text;
                    break;

                case 1:
                    key2Text.text = key.ToString();
                    ts.Data.Key2 = key2Text.text;
                    break;

                case 2:
                    key3Text.text = key.ToString();
                    ts.Data.Key3 = key3Text.text;
                    break;

                case 3:
                    key4Text.text = key.ToString();
                    ts.Data.Key4 = key4Text.text;
                    break;
            }

            SettingsManager.AddOrUpdate(ts);
        }

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
                ts = SettingsManager.SettingsStorage["Taiko"] as TaikoSettings;

            key1Text.text = ts.Data.Key1;
            key2Text.text = ts.Data.Key2;
            key3Text.text = ts.Data.Key3;
            key4Text.text = ts.Data.Key4;

            Application.targetFrameRate = tss.Data.FPSMenu;
            QualitySettings.vSyncCount = 0;
            Logger.Log($"Set FPS limit to {Application.targetFrameRate} and VSYNC {(QualitySettings.vSyncCount <= 0 ? "false" : "true")}");

            //Initialize static AutoInit Attributes
            System.Reflection.Assembly.GetExecutingAssembly().ActivateAttributeMethods<AutoInitAttribute>();

            SettingsManager.OnExit += (s, e) => Logger.Dispose();

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
            CheckForInput();
        }

        void FixedUpdate()
        {
        }

        void CheckForInput()
        {
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Logger.Log("Toggling Console");

                if (_devConsole != null)
                {
                    if (_devConsole.activeSelf)
                        _devConsole.SetActive(false);
                    else
                        _devConsole.SetActive(true);

                    Logger.Log("Toggled Console");
                }
                else
                {
                    _devConsole = Instantiate(DevConsolePrefab);
                    _devConsole.transform.SetParent(Canvas.transform);

                    RectTransform rt = _devConsole.GetComponent<RectTransform>();
                    rt.anchoredPosition3D = new Vector3(950, 347, 1.5f);

                    StartCoroutine(rt.MoveOverSeconds(rt.anchoredPosition3D, new Vector3(950, -383, 1.5f), 0.5f, true));
                    Logger.Log("Toggled Console");
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position -= new Vector3(5, 0, 0);
            }

            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position += new Vector3(5, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position += new Vector3(0, 5, 0);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 old = _devConsole.transform.position;
                Vector3 oldLocal = _devConsole.transform.localPosition;
                _devConsole.transform.position -= new Vector3(0, 5, 0);
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

            chart.Speed = 23;

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
                Logger.Log("Could not find option: " + optionStr, LogLevel.Error);
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
            Logger.Dispose();
#else
            Application.Quit();
#endif
        }
        
        void OnApplicationQuit()
        {
            SettingsManager.Exit();
        }
    }
}
