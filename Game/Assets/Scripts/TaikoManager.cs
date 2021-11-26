using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;
using Assets.Scripts.Controls;
using Assets.Scripts.IO.Charts;
using Assets.Scripts.TaikoGame;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class TaikoManager : MonoBehaviour
    {
        public static TaikoManager Instance;
        public static System.Random Random
        {
            get
            {
                if (_random == null)
                    _random = new System.Random();

                return _random;
            }
        }
        static System.Random _random;

        public bool StartPlaying;
        public int MultiplierValue;

        public int EscapeCheckDelayMS;

        public int TargetFPS = 1000; // Placeholder value (0 = unlimited)

        public GameObject DevConsolePrefab;
        public GameObject Canvas;

        //ChartFile _loadedChart;

        GameObject _devConsole;

        [SerializeField] NoteScroller _scroller;

        public TaikoManager()
        {
            Instance = this;
        }

        public static void OnSceneLoaded(ChartFile chart, DirectoryInfo chartDirectory, List<(string, float)> mods = null)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<TaikoManager>();

            GlobalConfig tss = (GlobalConfig)ConfigManager.GetOrLoadOrAdd<GlobalConfig>();
            Application.targetFrameRate = tss.FPSInGame;
            QualitySettings.vSyncCount = 0;

            Logger.Log($"Set FPS limit to {Application.targetFrameRate} and VSYNC {(QualitySettings.vSyncCount <= 0 ? "false" : "true")}");
            Instance._scroller.OnSceneLoaded(chartDirectory, chart, mods);
        }

        // Start is called before the first frame update
        void Start()
        {
            new Hotkey("ReturnToTitleScreen", KeyCode.Escape, HotkeyType.OnKeyDown)
                .OnInvoked += hk => OnTitleScreenKey();
            new Hotkey("ToggleDevConsole", KeyCode.KeypadMinus, HotkeyType.OnKeyDown)
                .OnInvoked += hk => OnDevConsoleKey();

            QualitySettings.vSyncCount = 0;
        }

        void OnTitleScreenKey()
        {
            Logger.Log("Switching to title screen");
            SceneManager.LoadScene("TitleScreen");
        }

        void OnDevConsoleKey()
        {
            GlobalConfig cfg = (GlobalConfig)ConfigManager.GetOrLoadOrAdd<GlobalConfig>();

            if (!cfg.IsDeveloperMode)
            {
                Logger.Log("Failed to open dev console due to dev mode being disabled", LogLevel.Warning);
                return;
            }

            Logger.Log("Opening dev console");
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
            }
        }

        void OnApplicationQuit()
        {
            ConfigManager.SaveConfigs();
        }
    }
}