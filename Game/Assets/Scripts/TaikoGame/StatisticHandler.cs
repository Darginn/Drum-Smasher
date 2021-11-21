using Assets.Scripts.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TaikoGame
{
    public class StatisticHandler : MonoBehaviour
    {
        public static StatisticHandler Instance { get; private set; }

        [SerializeField] Text _scoreText;
        [SerializeField] Text _comboText;
        [SerializeField] Text _accuracyText;

        [SerializeField] Text _soundOffsetText;

        [SerializeField] Text _key1Text;
        [SerializeField] Text _key2Text;
        [SerializeField] Text _key3Text;
        [SerializeField] Text _key4Text;
        CultureInfo _elGR = CultureInfo.CreateSpecificCulture("el-GR");

        public float Multiplier = 300f;

        ulong _currentScore;
        long _currentCombo;
        double _currentAccuracy;
        float _currentSoundOffset;

        long _currentKey1;
        long _currentKey2;
        long _currentKey3;
        long _currentKey4;

        ulong _misses;
        ulong _totalNotes;
        ulong _badHits;
        ulong _goodHits;

        public StatisticHandler()
        {
            Instance = this;
        }

        public ScoreStatistic GetScoreStatistics()
        {
            return new ScoreStatistic(_currentScore, _currentCombo, _currentAccuracy, _goodHits, _goodHits, _misses,
                                      perfect: _currentAccuracy == 100.0 ? true : false,
                                      rank: GetRank());
        }

        public void Reset()
        {
            _currentScore = 0u;
            _currentCombo = 0;
            _currentAccuracy = 0.0;
            _currentSoundOffset = 0f;

            _currentKey1 = 0;
            _currentKey2 = 0;
            _currentKey3 = 0;
            _currentKey4 = 0;

            _misses = 0u;
            _totalNotes = 0u;
            _badHits = 0u;
            _goodHits = 0u;
        }

        public void OnNoteMiss()
        {
            _totalNotes++;
            _currentCombo = 0;
            _misses++;

            RefreshAccuracy();
            RefreshVisuals();
        }

        public void OnNoteHit(bool goodHit, bool bigNote, bool isSegment = false, int scorePerHit = 30)
        {
            if (isSegment)
            {
                _currentScore += (uint)scorePerHit;
                RefreshVisuals();

                return;
            }

            if (goodHit)
            {
                _goodHits++;
                _currentCombo++;

                if (_currentCombo < 10)
                    _currentScore += (bigNote ? 600u : 300u) * (ulong)Multiplier;
                else
                    _currentScore += (ulong)((Math.Min((double)Math.Round(_currentCombo / 10.0, MidpointRounding.AwayFromZero), 10.0) * Math.Round(Multiplier, MidpointRounding.AwayFromZero))) * (ulong)Multiplier;
            }
            else
            {
                _badHits++;
                _currentCombo++;

                if (_currentCombo < 10)
                    _currentScore += (bigNote ? 600u : 300u) * (ulong)Multiplier;
                else
                    _currentScore += (ulong)((Math.Min((double)Math.Round(_currentCombo / 10.0, MidpointRounding.AwayFromZero), 10.0) * Math.Round(Multiplier, MidpointRounding.AwayFromZero))) * (ulong)Multiplier;
            }

            _totalNotes++;

            RefreshAccuracy();
            RefreshVisuals();
        }

        public void UpdateSoundOffset(float value)
        {
            _currentSoundOffset = value;
            _soundOffsetText.text = value.ToString("g", _elGR);
        }

        //TODO: for segments: <149: 1/16, 150-199 : 1/12, 200-249 : 1/8, 250-300 : 1/6

        /// <summary>
        /// Calculates the max score a slider can have
        /// </summary>
        /// <param name="duration">Map duration in seconds</param>
        /// <param name="totalNotes">Total Notes</param>
        /// <param name="sliderDuration">Slider duration in seconds</param>
        /// <param name="bpm">Beats per minute</param>
        /// <returns>Slider score cap</returns>
        public static double CalculateSliderScoreCap(double duration, int totalNotes, double sliderDuration, double bpm)
        {
            double t = duration / totalNotes;
            double l = sliderDuration / t;
            double x = bpm / 60.0;

            return Math.Truncate(l * Math.Pow(t, 10) * Math.Pow(x / 100, 10) / 15);
        }

        public void IncrementKey(HotkeyType hotkey)
        {
            switch (hotkey)
            {
                default:
                    throw new InvalidOperationException($"Invalid hotkey: {hotkey}");

                case HotkeyType.TaikoOuterLeft:
                    _currentKey1++;
                    _key1Text.text = _currentKey1.ToString();
                    break;

                case HotkeyType.TaikoInnerLeft:
                    _currentKey2++;
                    _key2Text.text = _currentKey2.ToString();
                    break;
                case HotkeyType.TaikoInnerRight:
                    _currentKey3++;
                    _key3Text.text = _currentKey3.ToString();
                    break;
                case HotkeyType.TaikoOuterRight:
                    _currentKey4++;
                    _key4Text.text = _currentKey4.ToString();
                    break;
            }
        }

        string GetRank()
        {
            string rank = "";

            if (_currentAccuracy == 100.0)
                return "SS";
            else if (_currentAccuracy > 95)
            {
                if (_misses == 0)
                    rank = "S";
                else
                    rank = "A";
            }
            else if (_currentAccuracy > 90)
                rank = "A";
            else if (_currentAccuracy > 85)
                rank = "B";
            else if (_currentAccuracy > 80)
                rank = "C";
            else
                rank = "D";

            return rank;
        }

        void RefreshAccuracy()
        {
            if (_totalNotes > 0)
                _currentAccuracy = (100.0 / _totalNotes) * (_badHits + _goodHits);
        }

        void RefreshVisuals()
        {
            _comboText.text = $"{_currentCombo} x";
            _accuracyText.text = $"{Math.Round(_currentAccuracy, 2, MidpointRounding.AwayFromZero)}%";
            _scoreText.text = _currentScore.ToString("g", _elGR);
        }
    }

    public class ScoreStatistic
    {
        public ulong Score { get; set; }
        public long Combo { get; set; }
        public double Accuracy { get; set; }
        public ulong PerfectFinisher { get; set; }
        public ulong GoodFinisher { get; set; }
        public ulong Misses { get; set; }
        public string Rank { get; set; }
        public bool Perfect { get; set; }

        public ScoreStatistic(ulong score, long combo, double accuracy,
                              ulong perfectFinisher, ulong goodFinisher,
                              ulong misses, string rank, bool perfect)
        {
            Score = score;
            Combo = combo;
            Accuracy = accuracy;
            PerfectFinisher = perfectFinisher;
            GoodFinisher = goodFinisher;
            Misses = misses;
            Rank = rank;
            Perfect = perfect;
        }


    }
}
