﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Game.Mods
{
    public class FadeInMod : BaseMod
    {
        public override string Name => _name;
        public override float Multiplier => _multiplier;

        [SerializeField] private string _name = "FadeIn";
        [SerializeField] private float _multiplier = 1.125f;
        [SerializeField] private float _hiddenSpeedMulti = 1.25f;
        [SerializeField] private float _hiddenDelay = 1f;

        public override void OnEnabled(NoteScroller scroller)
        {
            GetComponent<Collider2D>().enabled = true;
        }

        public override void OnDisabled(NoteScroller scroller)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            StartCoroutine(TurnVisibleCoroutine(collider.gameObject));
        }

        private IEnumerator TurnVisibleCoroutine(GameObject obj)
        {
            Note n = obj.GetComponent<Note>();

            SpriteRenderer renderer = n.Renderer;
            SpriteRenderer renderer2 = n.OverlayRenderer;

            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f);
            renderer2.color = new Color(renderer2.color.r, renderer2.color.g, renderer2.color.b, 0f);

            float delay = 0;

            while (renderer.color.a < 1)
            {
                if (obj == null ||
                    renderer == null ||
                    renderer2 == null)
                    yield return null;

                if (delay < _hiddenDelay)
                {
                    delay += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a + (_hiddenSpeedMulti * Time.deltaTime));
                renderer2.color = new Color(renderer2.color.r, renderer2.color.g, renderer2.color.b, renderer2.color.a + (_hiddenSpeedMulti * Time.deltaTime));

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
