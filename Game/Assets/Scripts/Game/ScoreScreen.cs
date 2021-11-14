using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Game
{
    public class ScoreScreen : MonoBehaviour
    {
        [SerializeField] Text _scoreText;
        [SerializeField] Text _accuracyText;
        [SerializeField] Text _perfectFinisherText;
        [SerializeField] Text _perfectText;
        [SerializeField] Text _goodFinisherText;
        [SerializeField] Text _goodText;
        [SerializeField] Text _missText;
        [SerializeField] Text _rankText;

        [SerializeField] GameObject _scoreScreen;

        CultureInfo _elGR = CultureInfo.CreateSpecificCulture("el-GR");

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

        public void Hide()
        {
            if (!_scoreScreen.activeSelf)
                return;

            _scoreScreen.SetActive(false);
        }

        public void Show()
        {
            if (_scoreScreen.activeSelf)
                return;

            _scoreScreen.SetActive(true);
        }
    }
}
