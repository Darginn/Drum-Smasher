using DrumSmasher.Notes;
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
        
        private Charts.Chart _loadedChart;

        public static void OnSceneLoaded(Charts.Chart chart, DirectoryInfo chartDirectory)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<GameManager>();
            Instance.NoteScroller.GameSound.Scroller = Instance.NoteScroller;
            
            Instance.NoteScroller.GameSound.LoadSong(chartDirectory.FullName + @"\" + chart.SoundFile);

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
        }
        
        void OnApplicationQuit()
        {
            Logger.Dispose();
        }
    }
}