using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    public class Hotkey
    {
        /// <summary>
        /// Invoked through <see cref="InvokeCheckKey"/>:
        /// <para></para>
        /// <inheritdoc cref="InvokeCheckKey"/>
        /// </summary>
        public event Action<Hotkey> OnChecked;
        /// <summary>
        /// Invoked through <see cref="InvokeCheckKeyDown"/>:
        /// <para></para>
        /// <inheritdoc cref="InvokeCheckKeyDown"/>
        /// </summary>
        public event Action<Hotkey> OnCheckedDown;
        /// <summary>
        /// Invoked through <see cref="InvokeCheckKeyUp"/>:
        /// <para></para>
        /// <inheritdoc cref="InvokeCheckKeyUp"/>
        /// </summary>
        public event Action<Hotkey> OnCheckedUp;

        /// <summary>
        /// The hotkey type
        /// </summary>
        public HotkeyType Type { get; }

        /// <summary>
        /// The main key
        /// <para>Cannot be any special key like shift, alt, etc.</para>
        /// </summary>
        public KeyCode Key
        {
            get => _k;
            set
            {
                switch (value)
                {
                    default:
                        _k = value;
                        break;

                    case KeyCode.None:
                        throw new InvalidOperationException("Key cannot be None");
                        
                    case KeyCode.LeftShift:
                    case KeyCode.RightShift:
                    case KeyCode.LeftControl:
                    case KeyCode.RightControl:
                    case KeyCode.LeftAlt:
                    case KeyCode.RightAlt:
                    case KeyCode.AltGr:
                        throw new InvalidOperationException("Key cannot be any special key");
                }
            }
        }
        /// <summary>
        /// The 1. special key
        /// <para>Can only be any special key like shift, alt, etc.</para>
        /// </summary>
        public KeyCode SpecialKey1
        {
            get => _sk1;
            set
            { 
                switch(value)
                {
                    default:
                        throw new InvalidOperationException("SpecialKey1 can only be a key like shift, alt etc.");

                    case KeyCode.LeftShift:
                    case KeyCode.RightShift:
                    case KeyCode.LeftControl:
                    case KeyCode.RightControl:
                    case KeyCode.LeftAlt:
                    case KeyCode.RightAlt:
                    case KeyCode.AltGr:
                        _sk1 = value;
                        break;
                }
            }
        }
        /// <summary>
        /// The 2. special key
        /// <para>Can only be any special key like shift, alt, etc.</para>
        /// </summary>
        public KeyCode SpecialKey2
        {
            get => _sk2;
            set
            {
                switch (value)
                {
                    default:
                        throw new InvalidOperationException("SpecialKey1 can only be a key like shift, alt etc.");

                    case KeyCode.LeftShift:
                    case KeyCode.RightShift:
                    case KeyCode.LeftControl:
                    case KeyCode.RightControl:
                    case KeyCode.LeftAlt:
                    case KeyCode.RightAlt:
                    case KeyCode.AltGr:
                        _sk2 = value;
                        break;
                }
            }
        }

        KeyCode _k;
        KeyCode _sk1;
        KeyCode _sk2;

        /// <param name="type"><see cref="Hotkey.Type"/></param>
        /// <param name="key"><see cref="Hotkey.Key"/></param>
        public Hotkey(HotkeyType type, KeyCode key)
        {
            Type = type;
            Key = key;
        }

        /// <param name="type"><see cref="Hotkey.Type"/></param>
        /// <param name="specialKey1"><see cref="Hotkey.SpecialKey1"/></param>
        /// <param name="key"><see cref="Hotkey.Key"/></param>
        public Hotkey(HotkeyType type, KeyCode specialKey1, KeyCode key) : this(type, key)
        {
            SpecialKey1 = specialKey1;
        }

        /// <param name="type"><see cref="Hotkey.Type"/></param>
        /// <param name="specialKey1"><see cref="Hotkey.SpecialKey1"/></param>
        /// <param name="specialKey2"><see cref="Hotkey.SpecialKey2"/></param>
        /// <param name="key"><see cref="Hotkey.Key"/></param>
        public Hotkey(HotkeyType type, KeyCode specialKey1, KeyCode specialKey2, KeyCode key) : this(type, specialKey1, key)
        {
            SpecialKey2 = specialKey2;
        }

        /// <summary>
        /// Checks if the key is pressed
        /// </summary>
        public bool CheckKey()
        {
            return CheckKey(new Func<KeyCode, bool>(Input.GetKey));
        }

        /// <summary>
        /// Checks if the key was pressed this frame
        /// </summary>
        public bool CheckKeyDown()
        {
            return CheckKey(new Func<KeyCode, bool>(Input.GetKeyDown));
        }

        /// <summary>
        /// Checks if the key was released this frame
        /// </summary>
        public bool CheckKeyUp()
        {
            return CheckKey(new Func<KeyCode, bool>(Input.GetKeyUp));
        }

        /// <summary>
        /// Calls <see cref="CheckKey"/> if the result is true <see cref="OnChecked"/> will be invoked
        /// </summary>
        /// <returns>Key is pressed</returns>
        public bool InvokeCheckKey()
        {
            if (CheckKey())
            {
                OnChecked?.Invoke(this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calls <see cref="CheckKeyDown"/> if the result is true <see cref="OnCheckedDown"/> will be invoked
        /// </summary>
        /// <returns>Key was pressed this frame</returns>
        public bool InvokeCheckKeyDown()
        {
            if (CheckKeyDown())
            {
                OnCheckedDown?.Invoke(this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calls <see cref="CheckKeyUp"/> if the result is true <see cref="OnCheckedUp"/> will be invoked
        /// </summary>
        /// <returns>Key was released this frame</returns>
        public bool InvokeCheckKeyUp()
        {
            if (CheckKeyUp())
            {
                OnCheckedUp?.Invoke(this);
                return true;
            }

            return false;
        }


        bool CheckKey(Func<KeyCode, bool> keyCheck)
        {
            if (!keyCheck(Key) ||
                (SpecialKey1 != KeyCode.None && !keyCheck(SpecialKey1)) ||
                (SpecialKey2 != KeyCode.None && !keyCheck(SpecialKey2)))
                return false;

            return true;
        }
    }

}
