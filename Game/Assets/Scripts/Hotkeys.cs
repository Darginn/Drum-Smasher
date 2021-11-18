using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace Assets.Scripts
{
    public static class HotkeyList
    {
        public static readonly string ToggleDevConsole = "DevConsoleT";
        public static readonly string ReturnToTitleScreen = "TSReturn";

        public static readonly string TaikoOuterLeft = "TKOL";
        public static readonly string TaikoInnerLeft = "TKIL";
        public static readonly string TaikoInnerRight = "TKIR";
        public static readonly string TaikoOuterRight = "TKOR";
    }

    public static class Hotkeys
    {
        static readonly Dictionary<string, Hotkey> _registeredKeys;

        static Hotkeys()
        {
            _registeredKeys = new Dictionary<string, Hotkey>();
        }

        public static void RegisterKey(Hotkey key)
        {
            RegisterKey(key, false);
        }

        public static void RegisterKey(Hotkey key, bool throwIfExists)
        {
            if (throwIfExists && _registeredKeys.ContainsKey(key.Id))
                throw new HotkeyAlreadyRegisteredException(key.Id);

            _registeredKeys[key.Id] = key;
        }

        public static void DeleteKey(string keyId)
        {
            _registeredKeys.Remove(keyId);
        }

        public static void DeleteKey(Hotkey key)
        {
            DeleteKey(key.Id);
        }

        public static Hotkey GetKey(string keyId)
        {
            _registeredKeys.TryGetValue(keyId, out Hotkey key);
            return key;
        }

        public static bool CheckKey(string keyId)
        {
            Hotkey key = GetKey(keyId);

            if (key == null)
                return false;

            return CheckKey(key);
        }

        public static bool CheckKeyDown(string keyId)
        {
            Hotkey key = GetKey(keyId);

            if (key == null)
                return false;

            return CheckKeyDown(key);
        }

        public static bool CheckKeyUp(string keyId)
        {
            Hotkey key = GetKey(keyId);

            if (key == null)
                return false;

            return CheckKeyUp(key);
        }

        public static bool CheckKey(Hotkey key)
        {
            return key.CheckKey();
        }

        public static bool CheckKeyDown(Hotkey key)
        {
            return key.CheckKeyDown();
        }

        public static bool CheckKeyUp(Hotkey key)
        {
            return key.CheckKeyUp();
        }
    }
    public class Hotkey
    {
        public string Id { get; }
        public Action OnPressed { get; set; }

        public KeyCode Key { get; set; }
        public KeyCode SpecialKey1 { get; set; }
        public KeyCode SpecialKey2 { get; set; }

        public Hotkey(string id, Action onPressed, KeyCode key)
        {
            Id = id; 
            OnPressed = onPressed;
            Key = key;
        }

        public Hotkey(string id, Action onPressed, KeyCode specialKey1, KeyCode key) : this(id, onPressed, key)
        {
            SpecialKey1 = specialKey1;
        }

        public Hotkey(string id, Action onPressed, KeyCode specialKey1, KeyCode specialKey2, KeyCode key) : this(id, onPressed, specialKey1, key)
        {
            SpecialKey2 = specialKey2;
        }

        public bool CheckKey()
        {
            return CheckKey(new Func<KeyCode, bool>(Input.GetKey));
        }

        public bool CheckKeyDown()
        {
            return CheckKey(new Func<KeyCode, bool>(Input.GetKeyDown));
        }

        public bool CheckKeyUp()
        {
            return CheckKey(new Func<KeyCode, bool>(Input.GetKeyUp));
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

    public class HotkeyAlreadyRegisteredException : Exception
    {
        public HotkeyAlreadyRegisteredException(string keyId) : base($"Hotkey '{keyId}' already exists")
        {

        }
    }
}
