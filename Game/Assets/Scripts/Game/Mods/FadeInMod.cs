using DrumSmasher.Assets.Scripts.Game.Notes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.Game.Mods
{
    public class FadeInMod : BaseMod
    {
        public override string Name => _name;
        public override float Multiplier => _multiplier;

        [SerializeField] private string _name = "FadeInMod";
        [SerializeField] private float _multiplier = 1.125f;
        [SerializeField] private Transform _end;

        [SerializeField] private Collider2D _firstCollider;
        [SerializeField] private Collider2D _secondCollider;

        private float _distance;

        public override void OnEnabled(NoteScroller scroller)
        {
            _firstCollider.enabled = true;
            _secondCollider.enabled = true;
        }

        public override void OnDisabled(NoteScroller scroller)
        {
            _firstCollider.enabled = false;
            _secondCollider.enabled = false;
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (_distance == 0f)
            {
                _distance = transform.position.x - _end.position.x;

                if (_distance < 0)
                    _distance *= -1;
            }

            
            StartCoroutine(TurnVisibleCoroutine(collider.gameObject));
        }

        private IEnumerator TurnVisibleCoroutine(GameObject obj)
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

                float currentPercentage = ((distOne * distance) / 100f);

                if (currentPercentage < renderer.color.a)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, currentPercentage);
                renderer2.color = new Color(renderer2.color.r, renderer2.color.g, renderer2.color.b, currentPercentage);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
