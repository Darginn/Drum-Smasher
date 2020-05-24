using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Game
{
    public class TaikoDrum : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _renderer;
        [SerializeField]
        private Sprite _triggerSprite;
        [SerializeField]
        private Sprite _defaultSprite;
        [SerializeField]
        private long _lightUpDurationMS = 50;

        private System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();

        void Start()
        {
        }

        void Update()
        {

        }

        public void Trigger()
        {
            if (_stopWatch.IsRunning)
                return;

            _stopWatch.Start();
            _renderer.sprite = _triggerSprite;
        }

        public void Reset()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();

            _renderer.sprite = _defaultSprite;
        }

    }
}
