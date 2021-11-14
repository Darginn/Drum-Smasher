using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Mods
{
    public class IgnoreColorMod : BaseMod
    {
        public override string Name => _name;
        public override float Multiplier => _multiplier;

        [SerializeField] string _name = "IgnoreColorMod";
        [SerializeField] float _multiplier = 0.5f;

        public override void OnEnabled(NoteScroller scroller)
        {
            scroller.IgnoreColor = true;
        }

        public override void OnDisabled(NoteScroller scroller)
        {
            scroller.IgnoreColor = false;
        }
    }
}
