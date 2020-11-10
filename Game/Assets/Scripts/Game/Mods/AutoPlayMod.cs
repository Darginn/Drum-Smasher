using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.Game.Mods
{
    public class AutoPlayMod : BaseMod
    {
        public override string Name => _name;
        public override float Multiplier => _multiplier;

        [SerializeField] string _name = "AutoPlayMod";
        [SerializeField] float _multiplier = 1.125f;

        public override void OnEnabled(NoteScroller scroller)
        {
            scroller.SetAutoPlay(true);
        }

        public override void OnDisabled(NoteScroller scroller)
        {
            scroller.SetAutoPlay(false);
        }
    }
}
