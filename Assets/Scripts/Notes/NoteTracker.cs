using System;
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

        public float MultiplierValue;
        public Text ScoreText;
        public Text ComboText;

        public NoteTracker()
        {

        }

        public void Hit(bool goodHit)
        {
            Logger.Log("Note hit", LogLevel.Trace);

            Combo++;

            if (Combo < 10)
                Score += 300;
            else
                Score += (long)(Math.Min((float)Math.Round(Combo / 10.0, MidpointRounding.AwayFromZero), 10f) * Math.Round(MultiplierValue, MidpointRounding.AwayFromZero));

            ComboText.text = "Combo: " + Combo.ToString();
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
        }
    }
}
