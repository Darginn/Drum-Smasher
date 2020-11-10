using DrumSmasher.Assets.Scripts.Game.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.Game.Mods
{
    public class FadeInStart : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            Note n = collider.gameObject.GetComponent<Note>();

            SpriteRenderer renderer = n.Renderer;
            SpriteRenderer renderer2 = n.OverlayRenderer;

            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0f);
            renderer2.color = new Color(renderer2.color.r, renderer2.color.g, renderer2.color.b, 0f);

        }
    }
}
