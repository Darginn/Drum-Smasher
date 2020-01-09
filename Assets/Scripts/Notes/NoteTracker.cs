﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Notes
{
    public class NoteTracker
    {
        public long Score { get; private set; }
        public int Combo { get; private set; }

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
        public int GoodHits { get; private set; }
        public int BadHits { get; private set; }
        public int Misses { get; private set; }

        public int Key1Hits { get; set; }
        public int Key2Hits { get; set; }
        public int Key3Hits { get; set; }
        public int Key4Hits { get; set; }

        public KeyCode Key1;
        public KeyCode Key2;
        public KeyCode Key3;
        public KeyCode Key4;

        public float MultiplierValue;
        public Text ScoreText;
        public Text ComboText;

        public Text Key1Text;
        public Text Key2Text;
        public Text Key3Text;
        public Text Key4Text;

        public NoteScroller Scroller;

        public NoteTracker()
        {

        }

        public void OnKeyHit(KeyCode code)
        {
            if (Key1 == code)
            {
                Key1Hits++;
                Key1Text.text = code.ToString() + ": " + Key1Hits;
            }
            else if (Key2 == code)
            {
                Key2Hits++;
                Key2Text.text = code.ToString() + ": " + Key2Hits;
            }
            else if (Key3 == code)
            {
                Key3Hits++;
                Key3Text.text = code.ToString() + ": " + Key3Hits;
            }
            else if (Key4 == code)
            {
                Key4Hits++;
                Key4Text.text = code.ToString() + ": " + Key4Hits;
            }
        }
        
        public void Hit(bool goodHit, bool bigNote)
        {
            Logger.Log("Note hit", LogLevel.Trace);

            Combo++;

            if (Combo < 10)
                Score += bigNote ? (goodHit ? 600 : 300) : 300;
            else
                Score += (long)(Math.Min((float)Math.Round(Combo / 10.0, MidpointRounding.AwayFromZero), 10f) * Math.Round(MultiplierValue, MidpointRounding.AwayFromZero));

            ComboText.text = Combo + "x";
            ScoreText.text = Score.ToString();
        }

        public void Miss()
        {
            Logger.Log("Note miss", LogLevel.Trace);

            Combo = 0;

            ComboText.text = "Combo: 0";
        }

        public void Reset()
        {
            ScoreText.text = "0";
            Combo = 0;
            ComboText.text = "";

            Key1Hits = 0;
            Key1Text.text = "";
            Key2Hits = 0;
            Key2Text.text = "";
            Key3Hits = 0;
            Key3Text.text = "";
            Key4Hits = 0;
            Key4Text.text = "";
        }
    }
}
