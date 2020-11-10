using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.GameInput
{
    public class HotKey
    {
        public KeyCode Key;
        public Action OnHit;
        public int DelayedActivationMS;
        public int DelayKeyCheckMS;
        public bool KeyActive;

        DateTime _activationTime;
        DateTime _nextKeyCheck;

        public HotKey(KeyCode key, Action onHit, int delayedActivationMS = 0, int delayKeyCheckMS = 250)
        {
            Key = key;
            OnHit = onHit;
            DelayedActivationMS = delayedActivationMS;
            DelayKeyCheckMS = delayKeyCheckMS;
            _nextKeyCheck = DateTime.Now;
            _activationTime = DateTime.MaxValue;
        }

        public virtual void CheckKey()
        {
            if (_nextKeyCheck > DateTime.Now)
                return;

            //Key is activated
            if (Input.GetKey(Key))
            {
                //No wait delay
                if (DelayedActivationMS <= 0)
                {
                    OnHit();
                    _nextKeyCheck = DateTime.Now.AddMilliseconds(DelayKeyCheckMS);
                    return;
                }
                else if (_activationTime == DateTime.MaxValue)
                {
                    _activationTime = DateTime.Now.AddMilliseconds(DelayedActivationMS);
                }
                else if (_activationTime <= DateTime.Now)
                {
                    OnHit();
                    _nextKeyCheck = DateTime.Now.AddMilliseconds(DelayKeyCheckMS);
                    _activationTime = DateTime.MaxValue;
                }
            }
            //Reset activation time incase we have a delay and key is not pressed anymore
            else if (KeyActive)
            {
                KeyActive = false;
                _activationTime = DateTime.MaxValue;
            }
        }
    }
}
