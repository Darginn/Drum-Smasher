using DrumSmasher.Notes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            Logger.Initialize("log.txt");
            StartPlaying = true;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFPS;
            TitleScreenKey = new HotKey(KeyCode.Escape, new Action(OnTitleScreenKey), EscapeCheckDelayMS);
        }
        

        private void OnTitleScreenKey()
        {
            Logger.Log("Switching to tile screen");
            SceneManager.SwitchScene(SceneManager.SCENE_TITLE);
        }

        // Update is called once per frame
        void Update()
        {
            TitleScreenKey.CheckKey();

            if (StartPlaying)
            {
                StartPlaying = false;
                const string testChartFolder = @"Assets\Charts\Sample Song\";
                Charts.Chart ch = Charts.ChartFile.Load(testChartFolder + "Test.Chart");

                if (ch == null)
                {
                    Logger.Log("Chart is null", LogLevel.ERROR);
                    return;
                }

                NoteScroller.Load(ch);
                NoteScroller.Tracker.MultiplierValue = MultiplierValue;
                NoteScroller.StartPlaying();
            }
            
            //if (StartPlaying)
            //{

            //    NoteScroller.Load(ch);
            //    NoteScroller.GameStart = true;
            //    StartPlaying = false;
            //}
        }
        
        void OnApplicationQuit()
        {
            Logger.Dispose();
        }
    }
}