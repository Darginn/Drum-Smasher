using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mods
{
    public class ModButton : MonoBehaviour
    {
        public bool Enabled { get; set; }
        public string ModName => _modName;
        public float Multiplier => _multiplier;

        [SerializeField] Image _image;
        [SerializeField] string _modName;
        [SerializeField] float _multiplier;

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
