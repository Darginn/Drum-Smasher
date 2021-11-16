using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;
using Assets.Scripts.Game;
using Assets.Scripts.GameInput;
using Assets.Scripts.IO.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
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

        public HotKey TitleScreenKey;
        public HotKey DevConsoleKey;

        public GameObject DevConsolePrefab;
        public GameObject Canvas;

        //ChartFile _loadedChart;

        GameObject _devConsole;

        [SerializeField] NoteScroller _scroller;

        public static void OnSceneLoaded(ChartFile chart, DirectoryInfo chartDirectory, List<(string, float)> mods = null)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<GameManager>();

            TitleScreenConfig tss = (TitleScreenConfig)ConfigManager.GetOrLoadOrAdd<TitleScreenConfig>();
            Application.targetFrameRate = tss.FPSInGame;
            QualitySettings.vSyncCount = 0;

            Logger.Log($"Set FPS limit to {Application.targetFrameRate} and VSYNC {(QualitySettings.vSyncCount <= 0 ? "false" : "true")}");
            Instance._scroller.LoadChart(chart, chartDirectory, true, mods);
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            TitleScreenKey = new HotKey(KeyCode.Escape, new Action(OnTitleScreenKey), EscapeCheckDelayMS);
            DevConsoleKey = new HotKey(KeyCode.KeypadMinus, new Action(OnDevConsoleKey));
            QualitySettings.vSyncCount = 0;
        }

        void OnTitleScreenKey()
        {
            Logger.Log("Switching to title screen");
            SceneManager.LoadScene("TitleScreen");
        }

        void OnDevConsoleKey()
        {
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

                RectTransform rt = _devConsole.GetComponent<RectTransform>();
                rt.anchoredPosition3D = new Vector3(950, 347, 1.5f);

                StartCoroutine(rt.MoveOverSeconds(rt.anchoredPosition3D, new Vector3(950, -383, 1.5f), 0.5f, true));
            }
        }

        // Update is called once per frame
        void Update()
        {
            TitleScreenKey.CheckKey();
        }

        void OnApplicationQuit()
        {
            ConfigManager.SaveConfigs();
        }
    }
}