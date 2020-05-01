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
        public bool TriggerMultipleTimes;

        public float HoldDuration;
        private float _holdingSince;

        public float Cooldown;
        private bool _isUnderCooldown;
        private float _timeSinceLastActivation;

        private bool _pressed;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (_pressed && Input.GetKeyUp(KeyToPress))
                KeyUp();

            //Check if cooldown is enabled
            if (Cooldown > 0)
            {
                if (_isUnderCooldown)
                {
                    _timeSinceLastActivation += Time.deltaTime;

                    if (_timeSinceLastActivation >= Cooldown) //We are not under cooldown anymore
                        _isUnderCooldown = false;
                    else //We are still under cooldown
                        return;
                }
                else //Enable cooldown
                {
                    _timeSinceLastActivation = 0;
                    _isUnderCooldown = true;
                }
            }

            if (HoldDuration > 0)
            {
                _holdingSince += Time.deltaTime;

                if (_holdingSince >= HoldDuration)
                {
                    TriggerKey();
                    return;
                }

                return;
            }

            if (TriggerMultipleTimes)
            {
                if (Input.GetKey(KeyToPress))
                    TriggerKey();
            }
            else
            {
                if (Input.GetKeyDown(KeyToPress))
                    TriggerKey();
            }
        }

        public void TriggerKey()
        {
            OnPressed?.Invoke();

            if (HoldDuration > 0)
                _holdingSince = 0;

            _pressed = true;
        }

        public void KeyUp()
        {
            OnPressedUp?.Invoke();
            _pressed = false;
        }
    }
}