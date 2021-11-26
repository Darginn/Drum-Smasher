using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TaikoGame.Modifiers
{
    [Flags]
    public enum ModIdentifier : long
    {
        None = -1L,
        Autoplay = 1L << 1, // The game automatically plays.
    }
}