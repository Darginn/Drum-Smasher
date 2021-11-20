using Assets.Scripts.Enums;
using Assets.Scripts.Game.Modifiers;
using Assets.Scripts.Game.Modifiers.Mods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Mods
{
    /// <summary>
    /// Entire class that controls the addition and removal of game mods.
    /// </summary>
    internal static class ModManager
    {
        /// <summary>
        /// The list of currently activated game modifiers.
        /// </summary>
        public static List<IGameplayModifier> CurrentModifiersList { get; } = new List<IGameplayModifier>();

        /// <summary>
        /// The current modifiers in ModId format
        /// </summary>
        public static ModIdentifier Mods
        {
            get
            {
                var mods = 0L;

                foreach (var mod in CurrentModifiersList)
                    mods += (long)mod.ModIdentifier;

                return (ModIdentifier)mods;
            }
        }

        /// <summary>
        /// Event emitted when mods have changed.
        /// </summary>
        public static event EventHandler<ModsChangedEventArgs> ModsChanged;

        /// <summary>
        /// Adds a gameplayModifier to our list, getting rid of any incompatible mods that are currently in there.
        /// </summary>
        public static void AddMod(ModIdentifier modIdentifier)
        {
            IGameplayModifier gameplayModifier;

            // Set the newMod based on the ModType that is coming in
            switch (modIdentifier)
            {
            
                case ModIdentifier.Autoplay:
                    gameplayModifier = new ModAutoplay();
                    break;

                default:
                    return;
            }

            // Remove incompatible mods.
            var incompatibleMods = CurrentModifiersList.FindAll(x => x.IncompatibleMods.Contains(gameplayModifier.ModIdentifier));
            incompatibleMods.ForEach(x => RemoveMod(x.ModIdentifier));

            // Remove the mod if it's already on.
            var alreadyOnMod = CurrentModifiersList.Find(x => x.ModIdentifier == gameplayModifier.ModIdentifier);
            if (alreadyOnMod != null)
                CurrentModifiersList.Remove(alreadyOnMod);

            // Add The Mod
            CurrentModifiersList.Add(gameplayModifier);
            gameplayModifier.InitializeMod();

            ModsChanged?.Invoke(typeof(ModManager), new ModsChangedEventArgs(Mods));
            Logger.Log($"Added mod: {gameplayModifier.ModIdentifier}.", LogLevel.Info);
        }

        /// <summary>
        /// Removes a gameplayModifier from our GameBase
        /// </summary>
        public static void RemoveMod(ModIdentifier modIdentifier)
        {
            try
            {
                // Try to find the removed gameplayModifier in the list
                var removedMod = CurrentModifiersList.Find(x => x.ModIdentifier == modIdentifier);

                // Remove the Mod
                CurrentModifiersList.Remove(removedMod);

                ModsChanged?.Invoke(typeof(ModManager), new ModsChangedEventArgs(Mods));
                Logger.Log($"Removed mod: {removedMod.ModIdentifier}.", LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log(e, LogLevel.Exception);
            }
        }

        /// <summary>
        /// Checks if a gameplayModifier is currently activated.
        /// </summary>
        /// <param name="modIdentifier"></param>
        /// <returns></returns>
        public static bool IsActivated(ModIdentifier modIdentifier) => CurrentModifiersList.Exists(x => x.ModIdentifier == modIdentifier);

        /// <summary>
        /// Removes all items from our list of mods
        /// </summary>
        public static void RemoveAllMods()
        {
            CurrentModifiersList.Clear();
            CheckModInconsistencies();

            ModsChanged?.Invoke(typeof(ModManager), new ModsChangedEventArgs(Mods));
            Logger.Log("Removed all modifiers", LogLevel.Info);
        }

        /// <summary>
        /// Adds speed mods from a given rate.
        /// </summary>
        /// <param name="rate"></param>
        [Obsolete("To be implemented")]
        public static void AddSpeedMods(float rate)
        {
            //TODO: make AddSpeedMods method
        }

        /// <summary>
        ///     Removes any speed mods from the game and resets the clock
        /// </summary>
        [Obsolete("To be implemented")]
        public static void RemoveSpeedMods()
        {
            //TODO: make RemoveSpeedMods method
        }

        /// <summary>
        /// Makes sure that the speed gameplayModifier selected matches up with the game clock and sets the correct one.
        /// </summary>
        [Obsolete("To be implemented")]
        private static void CheckModInconsistencies()
        {
            //TODO: make CheckModInconsistencies method
        }
    }
}