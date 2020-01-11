using DrumSmasher.Notes;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private bool _chartLoaded;
        private Charts.Chart _loadedChart;

        public static void OnSceneLoaded(Charts.Chart chart)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] objs = scene.GetRootGameObjects();

            Instance = objs.First(obj => obj.name.Equals("GameManager")).GetComponent<GameManager>();
            Instance.NoteScroller.Sound.LoadSong(chart.SoundFile);

            Instance.StartMap(chart);
        }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            StartMap(null);
        }

        private void StartMap(Charts.Chart chart)
        {
            Logger.Log("Starting Map");

            if (chart == null)
                _chartLoaded = false;
            else
            {
                _chartLoaded = true;
                _loadedChart = chart;
            }
            
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFPS;
            TitleScreenKey = new HotKey(KeyCode.Escape, new Action(OnTitleScreenKey), EscapeCheckDelayMS);

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
            TitleScreenKey.CheckKey();

            if (StartPlaying)
            {
                StartPlaying = false;
                
                if (!_chartLoaded)
                {
                    const string testChartFolder = @"Assets\Charts\Sample Song\";
                    _loadedChart = Charts.ChartFile.Load(testChartFolder + "testgenerated.Chart");

                    if (_loadedChart == null)
                    {
                        Logger.Log("Chart is null", LogLevel.ERROR);
                        return;
                    }
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