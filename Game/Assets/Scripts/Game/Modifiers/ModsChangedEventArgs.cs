using Assets.Scripts.Enums;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Modifiers
{
    public class ModsChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     The newly activated mods/
        /// </summary>
        public ModIdentifier Mods { get; }

        public ModsChangedEventArgs(ModIdentifier mods) => Mods = mods;
    }
}