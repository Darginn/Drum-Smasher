
﻿using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Debug
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private int _avgFrameRate;
        [SerializeField] private int _maxFrameRate;

        [SerializeField] private Text _avgFramerateText;
        [SerializeField] private Text _maxFramerateText;

        [SerializeField] private float _updateDelay = 0.25f;

        private int _oldFramerate;
        private int _oldMaxFramerate;

        private float _timeSinceLastUpdate;

        void FixedUpdate()
        {
            _timeSinceLastUpdate += Time.fixedDeltaTime;
            
            if (_timeSinceLastUpdate < _updateDelay)
                return;

            _avgFrameRate = (int)(Time.frameCount / Time.time);

            if (_avgFrameRate > _maxFrameRate)
                _maxFrameRate = _avgFrameRate;

            if (_avgFrameRate != _oldFramerate)
            {
                _avgFramerateText.text = _avgFrameRate.ToString();
                _oldFramerate = _avgFrameRate;
            }

            if (_maxFrameRate != _oldMaxFramerate)
            {
                _maxFramerateText.text = _maxFrameRate.ToString();
                _oldMaxFramerate = _maxFrameRate;
            }
        }

    }
}