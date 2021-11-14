using Assets.Scripts.Game.Notes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Mods
{
    public class FadeOutMod : BaseMod
    {
        public override string Name => _name;
        public override float Multiplier => _multiplier;

        [SerializeField] string _name = "FadeOutMod";
        [SerializeField] float _multiplier = 1.125f;
        [SerializeField] Transform _end;

        float _distance;

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
            if (_distance == 0f)
            {
                _distance = transform.position.x - _end.position.x;

                if (_distance < 0)
                    _distance *= -1;
            }

            
            StartCoroutine(TurnInvisibleCoroutine(collider.gameObject));
        }

        IEnumerator TurnInvisibleCoroutine(GameObject obj)
        {
            Note n = obj.GetComponent<Note>();

            SpriteRenderer renderer = n.Renderer;
            SpriteRenderer renderer2 = n.OverlayRenderer;

            while (n != null && n.transform.position.x >= _end.position.x)
            {
                _distance = transform.position.x - _end.position.x;

                if (_distance < 0)
                    _distance *= -1;

                float distOne = 100f / _distance;

                float distance = (n.transform.position.x - transform.position.x);

                if (distance < 0)
                    distance *= -1;

                float currentPercentage = 1f - ((distOne * distance) / 100f);

                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, currentPercentage);
                renderer2.color = new Color(renderer2.color.r, renderer2.color.g, renderer2.color.b, currentPercentage);

                yield return new WaitForEndOfFrame();
            }

            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f);
            renderer2.color = new Color(renderer2.color.r, renderer2.color.g, renderer2.color.b, 0f);
        }
    }
}
