using DrumSmasher.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DrumSmasher.GameInput
{
    public class ButtonController : MonoBehaviour
    {
        public KeyCode KeyToPress;
        public UnityEvent OnPressed;
        public UnityEvent OnPressedUp;
        public bool AutoPlay;
        public float AutoPlayDelay = 0.1f;
        public int KeyId;
        public StatisticHandler StatisticHandler;

        private float _holdingSince;


        private bool _pressed;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (_pressed)
            {
                _holdingSince += Time.deltaTime;

                if (AutoPlay)
                {
                    if (_holdingSince >= AutoPlayDelay)
                        KeyUp();
                }
                else if (Input.GetKeyUp(KeyToPress))
                    KeyUp();
            }

            if (Input.GetKeyDown(KeyToPress))
                TriggerKey();
        }

        public void TriggerKey()
        {
            OnPressed?.Invoke();
            _pressed = true;
            StatisticHandler?.IncrementKey(KeyId);
        }

        public void KeyUp()
        {
            OnPressedUp?.Invoke();
            _pressed = false;
            _holdingSince = 0f;
        }
    }
}