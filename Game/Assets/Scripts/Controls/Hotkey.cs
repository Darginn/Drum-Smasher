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
        /// Called through <see cref="Invoke"/>
        /// </summary>
        public event Action<Hotkey> OnInvoked;

        /// <summary>
        /// Key id, used to identify keys in the HotkeyManagher
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// If true the hotkey will automatically be checked upon each update in the unity mainthread
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Simply tells us if our key is registered in the HotkeyManager
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// The main key
        /// </summary>
        public KeyCode Key
        {
            get => _key;
            set
            {
                if (HotkeyManager.IsSpecialKey(value))
                    throw new InvalidOperationException("Special keys cannot be used (e.g.: Left Shift)");
                else if (value == KeyCode.None)
                    throw new OperationCanceledException("The main key cannot be set to None, use Hotkey.IsEnabled instead");

                _key = value;
            }
        }
        /// <summary>
        /// The first special key (e.g.: <see cref="KeyCode.LeftShift"/>), set to None to disable
        /// </summary>
        public KeyCode SpecialKey1
        {
            get => _skey1;
            set
            {
                if (!HotkeyManager.IsSpecialKey(value))
                    throw new InvalidOperationException("Only special keys can be used (e.g.: Left Shift)");

                _skey1 = value;
            }
        }
        /// <summary>
        /// The first special key (e.g.: <see cref="KeyCode.LeftShift"/>), set to None to disable
        /// </summary>
        public KeyCode SpecialKey2
        {
            get => _skey2;
            set
            {
                if (!HotkeyManager.IsSpecialKey(value))
                    throw new InvalidOperationException("Only special keys can be used (e.g.: Left Shift)");

                _skey2 = value;
            }
        }
        /// <summary>
        /// <inheritdoc cref="HotkeyType"/>
        /// </summary>
        public HotkeyType Type { get; set; }

        KeyCode _key;
        KeyCode _skey1;
        KeyCode _skey2;

        /// <param name="id">Id string used to identify the hotkey</param>
        /// <param name="isEnabled">Check on each unity update</param>
        /// <param name="register">Register the key after it has been created</param>
        public Hotkey(string id, KeyCode key, HotkeyType type, bool isEnabled = true, bool register = true)
        {
            Id = id;
            Key = key;
            Type = type;
            IsEnabled = isEnabled;

            if (register)
                Register();
        }

        /// <param name="id">Id string used to identify the hotkey</param>
        /// <param name="isEnabled">Check on each unity update</param>
        /// <param name="register">Register the key after it has been created</param>
        public Hotkey(string id, KeyCode key, KeyCode specialKey, HotkeyType type, bool isEnabled = true, bool register = true) 
            : this(id, key, type, isEnabled, register)
        {
            SpecialKey1 = specialKey;
        }

        /// <param name="id">Id string used to identify the hotkey</param>
        /// <param name="isEnabled">Check on each unity update</param>
        /// <param name="register">Register the key after it has been created</param>
        public Hotkey(string id, KeyCode key, KeyCode specialKey, KeyCode specialKey2, HotkeyType type, bool isEnabled = true, bool register = true) 
            : this(id, key, specialKey, type, isEnabled, register)
        {
            SpecialKey2 = specialKey2;
        }

        /// <param name="id">Id string used to identify the hotkey</param>
        /// <param name="isEnabled">Check on each unity update</param>
        /// <param name="register">Register the key after it has been created</param>
        public Hotkey(string id, KeyCode key, KeyCode specialKey, KeyCode specialKey2, HotkeyType type, Action<Hotkey> onInvoked, bool isEnabled = true, bool register = true) 
            : this(id, key, specialKey, specialKey2, type, isEnabled, register)
        {
            OnInvoked += onInvoked;
        }

        /// <summary>
        /// Registers the hotkey, equivalent to calling <see cref="HotkeyManager.RegisterKey(Hotkey)"/>
        /// </summary>
        public void Register()
        {
            HotkeyManager.RegisterKey(this);
        }

        /// <summary>
        /// Deregisters the hotkey, equivalent to calling <see cref="HotkeyManager.DeRegisterKey(Hotkey)"/>/<see cref="HotkeyManager.RegisterKey(Hotkey)"/>
        /// </summary>
        public void DeRegister()
        {
            HotkeyManager.DeRegisterKey(this);
        }

        /// <summary>
        /// Checks if the hotkey was pressed
        /// </summary>
        public bool Check()
        {
            if ((_skey1 != KeyCode.None && !Input.GetKey(_skey1)) ||
                (_skey2 != KeyCode.None && !Input.GetKey(_skey2)))
                return false;

            switch(Type)
            {
                default:
                case HotkeyType.OnKey:
                    return Input.GetKey(_key);

                case HotkeyType.OnKeyDown:
                    return Input.GetKeyDown(_key);

                case HotkeyType.OnKeyUp:
                    return Input.GetKeyUp(_key);
            }
        }

        /// <summary>
        /// Invokes the event <see cref="OnInvoked"/>
        /// </summary>
        public void Invoke()
        {
            OnInvoked?.Invoke(this);
        }

        /// <summary>
        /// Checks and invokes the hotkey (<see cref="Check"/>, <see cref="Invoke"/>)
        /// </summary>
        /// <returns></returns>
        public bool CheckAndInvoke()
        {
            if (Check())
            {
                Invoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all subscribed actions for this hotkey
        /// </summary>
        public void ClearSubscribedEvent()
        {
            OnInvoked = null;
        }
    }

}
