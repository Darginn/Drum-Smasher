using Assets.Scripts.Controls;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.TaikoGame
{
    public class ScoreScreen : MonoBehaviour
    {
        public static ScoreScreen Instance { get; private set; }

        [SerializeField] Text _scoreText;
        [SerializeField] Text _accuracyText;
        [SerializeField] Text _perfectFinisherText;
        [SerializeField] Text _perfectText;
        [SerializeField] Text _goodFinisherText;
        [SerializeField] Text _goodText;
        [SerializeField] Text _missText;
        [SerializeField] Text _rankText;

        CultureInfo _elGR = CultureInfo.CreateSpecificCulture("el-GR");

        public ScoreScreen()
        {
            Instance = this;
        }

        void Start()
        {
            Hide();
        }

        public void SetScoreStatistic(ScoreStatistic ss)
        {
            _scoreText.text = ss.Score.ToString("g", _elGR);
            _accuracyText.text = $"{Math.Round(ss.Accuracy, 3, MidpointRounding.AwayFromZero)} %";
            _perfectFinisherText.text = ss.PerfectFinisher.ToString();
            _perfectText.text = ss.Perfect.ToString();
            _goodFinisherText.text = ss.GoodFinisher.ToString();
            _goodText.text = ss.GoodFinisher.ToString();
            _missText.text = ss.Misses.ToString();
            _rankText.text = ss.Rank;
        }

        public void Reset()
        {
            Hide();

            _scoreText.text = "000.000.000";
            _accuracyText.text = "0 %";
            _perfectFinisherText.text = "0";
            _perfectText.text = "0";
            _goodFinisherText.text = "0";
            _goodText.text = "0";
            _missText.text = "0";
            _rankText.text = "SS";
        }

        public void ReturnToMenu()
        {
            Hotkeys.GetKey(HotkeyType.ReturnToTitleScreen)
                   .InvokeKey();
        }

        public void RestartMap()
        {
            NoteScroller.Instance.ReloadScene();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            gameObject.SetActive(false);
        }

        public void Show()
        {
            if (gameObject.activeSelf)
                return;

            gameObject.SetActive(true);
        }
    }
}

