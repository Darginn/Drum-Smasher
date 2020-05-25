using DrumSmasher.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Game.Mods
{
    public class BaseMod : MonoBehaviour
    {
        public virtual string Name { get; set; }
        public virtual float Multiplier { get; set; }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        public virtual void OnEnabled(NoteScroller scroller)
        {

        }

        public virtual void OnDisabled(NoteScroller scroller)
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {

        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {

        }
    }
}
