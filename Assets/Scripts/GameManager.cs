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

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            Logger.Initialize("log.txt");
            StartPlaying = true;
        }

        // Update is called once per frame
        void Update()
        {
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