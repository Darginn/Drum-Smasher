using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    public sealed class TimedHotkey : Hotkey
    {
        /// <summary>
        /// Time to press the key until it triggers, only works in combination with <see cref="InvokeKeyCheckTimed"/>
        /// </summary>
        public TimeSpan TriggerDelay { get; set; }

        float _timePressed;
        bool _isPressed;

        /// <param name="triggerDelay"><see cref="TriggerDelay"/></param>
        public TimedHotkey(HotkeyType type, KeyCode key, TimeSpan triggerDelay) : base(type, key)
        {
            TriggerDelay = triggerDelay;
        }


        /// <param name="triggerDelay"><see cref="TriggerDelay"/></param>
        public TimedHotkey(HotkeyType type, KeyCode specialKey1, KeyCode key, TimeSpan triggerDelay) : base(type, specialKey1, key)
        {
            TriggerDelay = triggerDelay;
        }

        /// <param name="triggerDelay"><see cref="TriggerDelay"/></param>
        public TimedHotkey(HotkeyType type, KeyCode specialKey1, KeyCode specialKey2, KeyCode key, TimeSpan triggerDelay) : base(type, specialKey1, specialKey2, key)
        {
            TriggerDelay = triggerDelay;
        }

        /// <summary>
        /// Checks if our key is pressed for the specified <see cref="TriggerDelay"/>
        /// </summary>
        /// <returns>Key was pressed long enough and was invoked</returns>
        public bool InvokeKeyCheckTimed()
        {
            // Our key was not pressed last frame, check if it's pressed in the current frame
            if (!_isPressed)
            {
                _isPressed = CheckKey();
            }
            else
            {
                // Check if our pressed time is longer than our delay
                if (_timePressed >= TriggerDelay.TotalSeconds)
                {
                    // Check if we are still pressing our key, if yes invoke it
                    if (InvokeCheckKey())
                    {
                        _isPressed = false;
                        _timePressed = 0;
                        return true;
                    }
                }
                // Our pressed time is smaller than our delay
                else
                {
                    // Check if we still pressing the key, if yes add the time since last frame
                    // if not just reset
                    if (CheckKey())
                    {
                        _timePressed += Time.deltaTime;
                    }
                    else
                    {
                        _isPressed = false;
                        _timePressed = 0;
                    }
                }
            }

            return false;
        }
    }
}
