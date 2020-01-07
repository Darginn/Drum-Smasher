using DrumSmasher.Note;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public const int SCORE_VALUE = 300;

        //Placeholder boolean
        public bool StartPlaying;

        public NoteScroll NoteScroller;
        
        public int MultiplierValue;
        public int MultiplierTracker;
        public int[] MultiplierThresholds;
        public int Combo;
        public int CurrentScore;

        public Text ScoreText;
        public Text MultiText;
        public Text ComboText;

        public float TotalNotes;
        //public float goodHits;
        public float MissHits;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            Logger.Initialize("log.txt");

            ScoreText.text = "0";
            Combo = 0;
            ComboText.text = "";
            MultiplierValue = 1;

            TotalNotes = FindObjectsOfType<NoteObject>().Length;
        }

        // Update is called once per frame
        void Update()
        {
            if (!StartPlaying)
            {
                StartPlaying = true;
                NoteScroller.GameStart = true;
            }
        }

        void OnApplicationQuit()
        {
            Logger.Dispose();
        }

        public void NoteHit()
        {
            //Logger.Log("Hit");

            Combo++;
            TotalNotes++;
            ComboText.text = Combo.ToString();

            if (MultiplierValue - 1 < MultiplierThresholds.Length)
            {
                MultiplierTracker++;

                if (MultiplierThresholds[MultiplierValue - 1] <= MultiplierTracker)
                {
                    MultiplierTracker = 0;
                    MultiplierValue++;
                }
            }

            CurrentScore += (SCORE_VALUE + (96 * (MultiplierValue - 1)));
            ScoreText.text = "" + CurrentScore;
        }

        /*public void NormalHit()
        {
            currentScore += (SCORE_VALUE + (96 * (multiplierValue - 1)));
            NoteHit();
            goodHits++;
        }*/

        

        public void NoteMissed()
        {
            Logger.Log("Miss");

            Combo = 0;
            MultiplierValue = 1;
            ComboText.text = "";

            MissHits++;
        }
    }
}