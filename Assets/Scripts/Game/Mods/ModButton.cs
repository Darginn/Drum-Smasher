using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Game.Mods
{
    public class ModButton : MonoBehaviour
    {
        public bool Enabled { get; private set; }
        public string ModName => _modName;
        public float Multiplier => _multiplier;

        [SerializeField] private Image _image;
        [SerializeField] private string _modName;
        [SerializeField] private float _multiplier;

        public void OnToggle()
        {
            if (Enabled)
            {
                _image.color = new Color(255, 255, 255, 1);
                Enabled = false;
            }
            else
            {
                _image.color = new Color(255, 0, 0, 1);
                Enabled = true;
            }
        }
    }
}
