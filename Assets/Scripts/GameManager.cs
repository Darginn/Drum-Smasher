using DrumSmasher.Game;
using DrumSmasher.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DrumSmasher
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
        private static System.Random _random;

        public bool StartPlaying;
        public int MultiplierValue;

        public int EscapeCheckDelayMS;

        public int TargetFPS = 1000; // Placeholder value (0 = unlimited)

        public HotKey TitleScreenKey;

        public GameObject DevConsolePrefab;
        public GameObject Canvas;

        //private Charts.Chart _loadedChart;

        private GameObject _devConsole;

        [SerializeField]
        private NoteScroller _scroller;

        public static void OnSceneLoaded(Charts.Chart chart, DirectoryInfo chartDirectory)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<GameManager>();
            Instance._scroller.LoadChart(chart, chartDirectory);
            Instance._scroller.Play();
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        private void OnTitleScreenKey()
        {
            Logger.Log("Switching to title screen");
            SceneManager.LoadScene("TitleScreen");
        }

        // Update is called once per frame
        void Update()
        {
            if (TitleScreenKey == null)
            {
                TitleScreenKey = new HotKey(KeyCode.Escape, new Action(OnTitleScreenKey), EscapeCheckDelayMS);
                QualitySettings.vSyncCount = 0;
            }

            TitleScreenKey.CheckKey();

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
        }

        private void OnApplicationQuit()
        {
            SettingsManager.Exit();
        }
    }
}