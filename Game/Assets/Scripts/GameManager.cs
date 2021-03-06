﻿using DrumSmasher.Assets.Scripts.Game;
using DrumSmasher.Assets.Scripts.GameInput;
using DrumSmasher.Assets.Scripts.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DrumSmasher.Assets.Scripts
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

        public GameObject DevConsolePrefab;
        public GameObject Canvas;

        //Charts.Chart _loadedChart;

        GameObject _devConsole;

        [SerializeField] NoteScroller _scroller;

        public static void OnSceneLoaded(Charts.Chart chart, DirectoryInfo chartDirectory, List<(string, float)> mods = null)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<GameManager>();

            TitleScreenSettings tss = (TitleScreenSettings)SettingsManager.SettingsStorage["TitleScreen"];
            Application.targetFrameRate = tss.Data.FPSInGame;
            QualitySettings.vSyncCount = 0;

            Logger.Log($"Set FPS limit to {Application.targetFrameRate} and VSYNC {(QualitySettings.vSyncCount <= 0 ? "false" : "true")}");
            Instance._scroller.LoadChart(chart, chartDirectory, true, mods);
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        void OnTitleScreenKey()
        {
            Logger.Log("Switching to title screen");
            SceneManager.LoadScene("TitleScreen");
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
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

        void OnApplicationQuit()
        {
            SettingsManager.Exit();
        }
    }
}