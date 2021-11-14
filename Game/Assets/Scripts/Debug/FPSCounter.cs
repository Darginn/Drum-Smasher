
﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Debug
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] int _avgFrameRate;
        [SerializeField] int _maxFrameRate;

        [SerializeField] Text _avgFramerateText;
        [SerializeField] Text _maxFramerateText;

        [SerializeField] float _updateDelay = 0.25f;

        int _oldFramerate;
        int _oldMaxFramerate;

        float _timeSinceLastUpdate;

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