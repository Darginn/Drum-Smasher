using Assets.Scripts.Enums;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Modifiers.Mods
{
    public class ModAutoplay : IGameplayModifier
    {
        public string Name { get; set; } = "Autoplay";

        public ModIdentifier ModIdentifier { get; set; } = ModIdentifier.Autoplay;

        public ModType Type { get; set; } = ModType.Special;

        public string Description { get; set; } = "Take a break and watch the game play for you.";

        public bool Ranked { get; set; } = false;

        public bool AllowedInMultiplayer { get; set; } = false;

        public bool OnlyMultiplayerHostCanCanChange { get; set; }

        public ModIdentifier[] IncompatibleMods { get; set; } =
        {
            // nofail? lol
        };

        public void InitializeMod()
        {
        }
    }
}