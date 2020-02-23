﻿using DrumSmasher.Notes;
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

        public NoteScroller NoteScroller;
        public int MultiplierValue;

        public int EscapeCheckDelayMS;

        public int TargetFPS = 1000; // Placeholder value (0 = unlimited)

        public HotKey TitleScreenKey;

        public GameObject DevConsolePrefab;
        public GameObject Canvas;

        private Charts.Chart _loadedChart;
        
        private GameObject _devConsole;


        public static void OnSceneLoaded(Charts.Chart chart, DirectoryInfo chartDirectory)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<GameManager>();
            Instance.NoteScroller.GameSound.Scroller = Instance.NoteScroller;
            
            Instance.NoteScroller.GameSound.LoadSong(chartDirectory.FullName + @"\" + chart.SoundFile);

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
                Instance.NoteScroller.AutoPlay = true;
#endif

            Instance.StartMap(chart);
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        private void StartMap(Charts.Chart chart)
        {
            Logger.Log("Starting Map");

            _loadedChart = chart;
            
            StartPlaying = true;
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
                Application.targetFrameRate = TargetFPS;
            }

            TitleScreenKey.CheckKey();

            if (StartPlaying)
            {
                StartPlaying = false;
                
                if (_loadedChart == null)
                {
                    Logger.Log("No chart found");
                    return;
                }

                NoteScroller.Load(_loadedChart);
                NoteScroller.Tracker.MultiplierValue = MultiplierValue;
                NoteScroller.StartPlaying();
            }

            if (TitleScreenManager.TaikoSettings.Data.ApproachRate != NoteScroller.ApproachRate)
                NoteScroller.ApproachRate = TitleScreenManager.TaikoSettings.Data.ApproachRate;

#if !UNITY_EDITOR
            NoteScroller.AutoPlay = TitleScreenManager.TaikoSettings.Data.Autoplay;
#endif


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
            Logger.Dispose();
        }
    }
}