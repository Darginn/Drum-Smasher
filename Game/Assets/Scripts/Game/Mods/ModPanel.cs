using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Assets.Scripts.Game.Mods
{
    public class ModPanel : MonoBehaviour
    {
        [SerializeField] List<ModButton> _modButtons;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>ModName, Multiplier</returns>
        public List<(string, float)> GetMods()
        {
            List<(string, float)> mods = new List<(string, float)>();

            for (int i = 0; i < _modButtons.Count; i++)
            {
                if (!_modButtons[i].Enabled)
                    continue;

                mods.Add((_modButtons[i].ModName, _modButtons[i].Multiplier));
            }

            if (mods.Count == 0)
                return null;

            return mods;
        }
    }
}
