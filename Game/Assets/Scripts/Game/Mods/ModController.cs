using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.Game.Mods
{
    public class ModController : MonoBehaviour
    {
        public GameObject ModObject;
        public BaseMod BaseMod;
        public string Name => BaseMod.Name;
    }
}
