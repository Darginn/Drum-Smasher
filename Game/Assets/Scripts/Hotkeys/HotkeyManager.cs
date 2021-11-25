using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Hotkeys2
{
    /// <summary>
    /// Controls all hotkeys, this class is automatically initialized upon accessing it for the first time
    /// </summary>
    public class HotkeyManager : MonoBehaviour
    {
        /// <summary>
        /// The active instance
        /// </summary>
        public static HotkeyManager Instance { get; private set; }

        static Dictionary<string, Hotkey> _registeredKeys;
        static bool _isInitialized;

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            _registeredKeys = new Dictionary<string, Hotkey>();
            GameObject gObj = new GameObject("HotkeyManager", typeof(HotkeyManager));

            DontDestroyOnLoad(gObj);
        }

        /// <summary>
        /// Registers a <see cref="Hotkey"/>, if the key already exists it is replaced
        /// </summary>
        /// <returns><para>True - key already exists and was replaced</para>
        /// <para>False - key does not exist and was added</para></returns>
        public static bool RegisterKey(Hotkey key)
        {
            bool wasOverwritten = _registeredKeys.ContainsKey(key.Id);
            _registeredKeys[key.Id] = key;

            return wasOverwritten;
        }

        /// <summary>
        /// <inheritdoc cref="DeRegisterKey(string)"/>
        /// </summary>
        /// <returns>Key was registered and then deregistered</returns>
        public static bool DeRegisterKey(Hotkey key)
        {
            return DeRegisterKey(key.Id);
        }

        /// <summary>
        /// Deregisters a hotkey with a specific id
        /// </summary>
        /// <param name="id"><see cref="Hotkey.Id"/></param>
        /// <returns>Key was registered and then deregistered</returns>
        public static bool DeRegisterKey(string id)
        {
            return _registeredKeys.Remove(id);
        }

        /// <summary>
        /// Checks if a specific <see cref="KeyCode"/> is a special key (e.g.: <see cref="KeyCode.LeftShift"/>)
        /// </summary>
        public static bool IsSpecialKey(KeyCode key)
        {
            switch (key)
            {
                default:
                    return false;

                case KeyCode.LeftAlt:
                case KeyCode.LeftShift:
                case KeyCode.LeftControl:
                case KeyCode.RightAlt:
                case KeyCode.RightShift:
                case KeyCode.RightControl:
                    return true;
            }
        }

        /// <summary>
        /// Clears all currently registered hotkeys
        /// </summary>
        public static void ClearKeys()
        {
            _registeredKeys.Clear();
        }

        void Update()
        {
            Hotkey[] tempKeys = _registeredKeys.Values.ToArray();

            for (int i = 0; i < tempKeys.Length; i++)
            {
                Hotkey key = tempKeys[i];

                if (key.IsEnabled)
                    key.CheckAndInvoke();
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            ClearKeys();
        }
    }
}
