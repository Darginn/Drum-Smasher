using Assets.Scripts.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace Assets.Scripts.Controls
{
    /// <summary>
    /// Manages hotkeys
    /// </summary>
    public static class Hotkeys
    {
        static readonly Dictionary<HotkeyType, Hotkey> _registeredKeys;

        static Hotkeys()
        {
            _registeredKeys = new Dictionary<HotkeyType, Hotkey>();
        }

        /// <summary>
        /// Registers a new <see cref="Hotkey"/> or overrides an existing one
        /// </summary>
        /// <param name="key"></param>
        public static Hotkey RegisterKey(Hotkey key)
        {
            return RegisterKey(key, false);
        }

        /// <summary>
        /// Registers a new <see cref="Hotkey"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="throwIfExists">Throw an exception if key already exists
        ///     <para>If false this will override the <see cref="Hotkey"/> if it already exists</para>
        /// </param>
        public static Hotkey RegisterKey(Hotkey key, bool throwIfExists)
        {
            if (throwIfExists && _registeredKeys.ContainsKey(key.Type))
                throw new HotkeyAlreadyRegisteredException(key.Type);

            return _registeredKeys[key.Type] = key;
        }

        /// <summary>
        /// Deletes an existing <see cref="Hotkey"/>
        /// </summary>
        public static void DeleteKey(HotkeyType keyType)
        {
            _registeredKeys.Remove(keyType);
        }
        /// <summary>
        /// Deletes an existing <see cref="Hotkey"/>
        /// </summary>
        public static void DeleteKey(Hotkey key)
        {
            DeleteKey(key.Type);
        }

        /// <summary>
        /// Gets a specific <see cref="Hotkey"/>
        /// </summary>
        /// <returns>Returns the <see cref="Hotkey"/> or null if not found</returns>
        public static Hotkey GetKey(HotkeyType keyType)
        {
            _registeredKeys.TryGetValue(keyType, out Hotkey key);
            return key;
        }

        /// <summary>
        /// <inheritdoc cref="Hotkey.CheckKey"/>
        /// </summary>
        public static bool CheckKey(HotkeyType keyType)
        {
            Hotkey key = GetKey(keyType);

            if (key == null)
                return false;

            return key.CheckKey();
        }
        /// <summary>
        /// <inheritdoc cref="Hotkey.CheckKeyDown"/>
        /// </summary>
        public static bool CheckKeyDown(HotkeyType keyType)
        {
            Hotkey key = GetKey(keyType);

            if (key == null)
                return false;

            return key.CheckKeyDown();
        }
        /// <summary>
        /// <inheritdoc cref="Hotkey.CheckKeyUp"/>
        /// </summary>
        public static bool CheckKeyUp(HotkeyType keyType)
        {
            Hotkey key = GetKey(keyType);

            if (key == null)
                return false;

            return key.CheckKeyUp();
        }

        /// <summary>
        /// <inheritdoc cref="Hotkey.InvokeCheckKey"/>
        /// </summary>
        public static bool InvokeCheckKey(HotkeyType keyType)
        {
            Hotkey key = GetKey(keyType);

            if (key == null)
                return false;

            return key.InvokeCheckKey();
        }
        /// <summary>
        /// <inheritdoc cref="Hotkey.InvokeCheckKeyDown"/>
        /// </summary>
        public static bool InvokeCheckKeyDown(HotkeyType keyType)
        {
            Hotkey key = GetKey(keyType);

            if (key == null)
                return false;

            return key.InvokeCheckKeyDown();
        }
        /// <summary>
        /// <inheritdoc cref="Hotkey.InvokeCheckKeyUp"/>
        /// </summary>
        public static bool InvokeCheckKeyUp(HotkeyType keyType)
        {
            Hotkey key = GetKey(keyType);

            if (key == null)
                return false;

            return key.InvokeCheckKeyUp();
        }
    }

}
