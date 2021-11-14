using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameInput
{
    //TODO: replace old hotkeys with KeyScanner
    /// <summary>
    /// Used to capture specific hotkeys
    /// </summary>
    public class KeyScanner : MonoBehaviour
    {
        public event Action OnKeyUp;
        public event Action OnKeyDown;
        public event Action OnKey;

        public List<KeyCode> Keys => _keys;

        /// <summary>
        /// Decides if we should scan each <see cref="FixedUpdate"/> or <see cref="Update"/>
        /// <para>0 = <see cref="Update"/></para>
        /// <para>1 = <see cref="FixedUpdate"/></para>
        /// </summary>
        public int UpdateType
        {
            get => _updateType;
            set
            {
                _updateType = Math.Max(0, Math.Min(1, value));
            }
        }
        /// <summary>
        /// If not <see cref="TimeSpan.Zero"/> then wait with <see cref="OnKeyDown"/> until delay has passed
        /// </summary>
        public TimeSpan TriggerDelay
        {
            get => _triggerDelay;
            set => _triggerDelay = value;
        }

        [SerializeField] int _updateType;
        [SerializeField] bool _keyDown;
        [SerializeField] List<KeyCode> _keys = new List<KeyCode>();
        [SerializeField] TimeSpan _triggerDelay;

        //see TriggerDelay
        float _timePassed;

        void Update()
        {
            if (_updateType != 0)
                return;

            ScanKeys();
        }

        void FixedUpdate()
        {
            if (_updateType != 1)
                return;

            ScanKeys();
        }

        /// <summary>
        /// Scans for user input for keys from <see cref="Keys"/>
        /// </summary>
        void ScanKeys()
        {
            if (_keys.Count == 0)
                return;

            bool keyPressed = false;
            for (int i = 0; i < _keys.Count; i++)
            {
                if (Input.GetKey(_keys[i]))
                {
                    keyPressed = true;

                    //Do not do anything if we have a delay and haven't passed it yet
                    if (!_triggerDelay.Equals(TimeSpan.Zero) &&
                        _timePassed < _triggerDelay.TotalSeconds)
                    {
                        _timePassed += Time.deltaTime;
                        return;
                    }

                    if (!_keyDown)
                    {
                        _keyDown = true;
                        OnKeyDown?.Invoke();
                    }
                    else
                        OnKey?.Invoke();

                    //Don't trigger multiple times
                    break;
                }
            }

            if (!keyPressed && _keyDown)
            {
                _timePassed = 0f;
                _keyDown = false;
                OnKeyUp?.Invoke();
            }
        }
    }
}
