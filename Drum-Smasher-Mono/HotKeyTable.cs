using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono
{
    public enum HotKey
    {
        FullScreen,
        ExitGame,

        OuterLeftDrum,
        InnerLeftDrum,
        OuterRightDrum,
        InnerRightDrum,
        PauseGame
    }

    public static class HotKeyTable
    {
        /// <summary>
        /// Contains all registered hotkeys
        /// </summary>
        public static readonly Dictionary<HotKey, (Keys, Keys?)> Table = new Dictionary<HotKey, (Keys, Keys?)>()
        {
            // Client Controls (always active, checked inside of GameClient.Update)
            { HotKey.FullScreen, (Keys.LeftControl, Keys.Enter) },
            { HotKey.ExitGame, (Keys.LeftControl, Keys.Escape) },


            // Game controls (only active while a map is playing)
            { HotKey.OuterLeftDrum, (Keys.A, null) },
            { HotKey.InnerLeftDrum, (Keys.S, null) },
            { HotKey.OuterRightDrum, (Keys.J, null) },
            { HotKey.InnerRightDrum, (Keys.K, null) },
            { HotKey.PauseGame, (Keys.Escape, null) }
        };

        /// <summary>
        /// Registers a hotkey with the specified keys, you can only have one combination at a time and setting a new one will override the old hotkey
        /// </summary>
        public static void RegisterHotKey(HotKey key, Keys key1)
        {
            RegisterHotKey(key, key1, null);
        }

        /// <summary>
        /// Registers a hotkey with the specified keys, you can only have one combination at a time and setting a new one will override the old hotkey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key1">If you are using 2 keys use this for the shift/ctrl key</param>
        /// <param name="key2">Only used if <paramref name="key1"/> is shift/ctrl</param>
        public static void RegisterHotKey(HotKey key, Keys key1, Keys? key2)
        {
            if (key2.HasValue)
            {
                switch (key2)
                {
                    case Keys.LeftControl:
                    case Keys.RightControl:
                    case Keys.LeftShift:
                    case Keys.RightShift:
                        Keys temp1 = key1;
                        key1 = key2.Value;
                        key2 = temp1;
                        break;
                }
            }

            Table[key] = (key1, key2);
        }

        /// <summary>
        /// Deletes/Unsets a hotkey
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteHotKey(HotKey key)
        {
            Table.Remove(key);
        }

        /// <summary>
        /// Checks if a hotkey is currently pressed
        /// </summary>
        public static bool IsKeyDown(HotKey key)
        {
            KeyboardState kstate = Keyboard.GetState();
            return IsKeyDown(key, ref kstate);
        }

        /// <summary>
        /// Checks if a hotkey is currently pressed
        /// </summary>
        public static bool IsKeyDown(HotKey key, ref KeyboardState kstate)
        {
            bool lctrl = kstate.IsKeyDown(Keys.LeftControl);
            bool rctrl = kstate.IsKeyDown(Keys.RightControl);
            bool lshift = kstate.IsKeyDown(Keys.LeftShift);
            bool rshift = kstate.IsKeyDown(Keys.RightShift);

            if (Table.TryGetValue(key, out (Keys, Keys?) hotkey))
            {
                bool getKey2 = false;

                switch (hotkey.Item1)
                {
                    case Keys.LeftShift:
                        if (!lshift)
                            return false;
                        getKey2 = true;
                        break;
                    case Keys.RightShift:
                        if (!rshift)
                            return false;
                        getKey2 = true;
                        break;

                    case Keys.LeftControl:
                        if (!lctrl)
                            return false;
                        getKey2 = true;
                        break;
                    case Keys.RightControl:
                        if (!rctrl)
                            return false;
                        getKey2 = true;
                        break;
                }

                Keys keyToCheck = hotkey.Item1;

                if (getKey2)
                {
                    if (!hotkey.Item2.HasValue)
                        return false;

                    keyToCheck = hotkey.Item2.Value;
                }

                if (kstate.IsKeyDown(keyToCheck))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a hotkey is currently not pressed
        /// </summary>
        public static bool IsKeyUp(HotKey key)
        {
            KeyboardState kstate = Keyboard.GetState();
            return IsKeyUp(key, ref kstate);
        }

        /// <summary>
        /// Checks if a hotkey is currently not pressed
        /// </summary>
        public static bool IsKeyUp(HotKey key, ref KeyboardState kstate)
        {
            bool lctrl = kstate.IsKeyUp(Keys.LeftControl);
            bool rctrl = kstate.IsKeyUp(Keys.RightControl);
            bool lshift = kstate.IsKeyUp(Keys.LeftShift);
            bool rshift = kstate.IsKeyUp(Keys.RightShift);

            if (Table.TryGetValue(key, out (Keys, Keys?) hotkey))
            {
                bool getKey2 = false;

                switch (hotkey.Item1)
                {
                    case Keys.LeftShift:
                        if (!lshift)
                            return false;
                        getKey2 = true;
                        break;
                    case Keys.RightShift:
                        if (!rshift)
                            return false;
                        getKey2 = true;
                        break;

                    case Keys.LeftControl:
                        if (!lctrl)
                            return false;
                        getKey2 = true;
                        break;
                    case Keys.RightControl:
                        if (!rctrl)
                            return false;
                        getKey2 = true;
                        break;
                }

                Keys keyToCheck = hotkey.Item1;

                if (getKey2)
                {
                    if (!hotkey.Item2.HasValue)
                        return false;

                    keyToCheck = hotkey.Item2.Value;
                }

                if (kstate.IsKeyUp(keyToCheck))
                    return true;
            }

            return false;
        }
    }
}
