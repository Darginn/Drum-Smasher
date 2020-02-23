using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Notes
{
    public class NoteTracker : MonoBehaviour
    {
        public long Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                ScoreText.text = value.ToString();
            }
        }
        public int Combo
        {
            get
            {
                return _combo;
            }
            set
            {
                _combo = value;
                ComboText.text = value + "x";
            }
        }
        
        public int GoodHits;
        public int BadHits;
        public int Misses;
        
        public int TotalNotes
        {
            get
            {
                return GoodHits + BadHits + Misses;
            }
        }
        public int TotalHits
        {
            get
            {
                return GoodHits + BadHits;
            }
        }

        public int Key1Hits
        {
            get
            {
                return _key1Hits;
            }
            set
            {
                _key1Hits = value;
                Key1Text.text = $"{Key1.ToString()}: {value}";
            }
        }
        public int Key2Hits
        {
            get
            {
                return _key2Hits;
            }
            set
            {
                _key2Hits = value;
                Key2Text.text = $"{Key2.ToString()}: {value}";
            }
        }
        public int Key3Hits
        {
            get
            {
                return _key3Hits;
            }
            set
            {
                _key3Hits = value;
                Key3Text.text = $"{Key3.ToString()}: {value}";
            }
        }
        public int Key4Hits
        {
            get
            {
                return _key4Hits;
            }
            set
            {
                _key4Hits = value;
                Key4Text.text = $"{Key4.ToString()}: {value}";
            }
        }

        public double Accuracy
        {
            get
            {
                return _accuracy;
            }
            set
            {
                _accuracy = value;
                AccuracyText.text = string.Format("{0:0.00}", value) + "%";
            }
        }

        public long FirstOffsetNoteHit;
        public double MultiplierValue;

        public KeyCode Key1;
        public KeyCode Key2;
        public KeyCode Key3;
        public KeyCode Key4;

        public Text ScoreText;
        public Text ComboText;
        public Text AccuracyText;
        public Text OffsetText;
        public GameTime GameTime;

        public Text Key1Text;
        public Text Key2Text;
        public Text Key3Text;
        public Text Key4Text;

        private long _score;
        private int _combo;

        private int _key1Hits;
        private int _key2Hits;
        private int _key3Hits;
        private int _key4Hits;

        private double _accuracy;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (GameTime != null)
                OffsetText.text = GameTime.ElapsedMilliseconds + " ms";
        }

        /// <summary>
        /// Tracks a note hit
        /// </summary>
        /// <param name="type">Good/Bad/Miss</param>
        /// <param name="bigNote">Big note?</param>
        public void NoteHit(HitType type, bool bigNote = false, KeyCode? key = null, KeyCode? key2 = null)
        {
            if (type == HitType.Miss)
            {
                Combo = 0;
                Misses++;
                Accuracy = (100.0 / TotalNotes) * ((BadHits * 0.5) + GoodHits);

                return;
            }

            if (Combo < 10)
                Score += bigNote ? (type == HitType.GoodHit ? 600 : 300) : 300;
            else
                Score += (long)(Math.Min((float)Math.Round(Combo / 10.0, MidpointRounding.AwayFromZero), 10.0) * Math.Round(MultiplierValue, MidpointRounding.AwayFromZero));

            if (type == HitType.GoodHit)
                GoodHits++;

            Combo++;

            Accuracy = (100.0 / TotalNotes) * ((BadHits * 0.5) + GoodHits);
            
            if (key.HasValue)
                KeyHit(key.Value);
            if (key2.HasValue)
                KeyHit(key2.Value);
        }

        /// <summary>
        /// Tracks a key hit
        /// </summary>
        /// <param name="key">Pressed key</param>
        public void KeyHit(KeyCode key)
        {
            if (key == Key1)
                Key1Hits++;
            else if (key == Key2)
                Key2Hits++;
            else if (key == Key3)
                Key3Hits++;
            else if (key == Key4)
                Key4Hits++;
            else
                Logger.Log("Key ignored since no play key: " + key.ToString(), LogLevel.WARNING);
        }

        /// <summary>
        /// Resets the tracker
        /// </summary>
        public void Reset()
        {
            Logger.Log("Resetting tracker", LogLevel.Trace);

            FirstOffsetNoteHit = 0;

            Key1Text.text = "";
            Key2Text.text = "";
            Key3Text.text = "";
            Key4Text.text = "";

            Score = 0;
            Combo = 0;
            Accuracy = 0;

            GoodHits = 0;
            BadHits = 0;
            Misses = 0;
        }

        public enum HitType
        {
            Miss,
            BadHit,
            GoodHit,
        }
    }
}
