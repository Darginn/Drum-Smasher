using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.Game.Notes
{
    public class NoteHitBox : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject == null)
                return;

            Note note = collider.gameObject.GetComponent<Note>();

            if (note == null)
            {
                NoteSegment segment = collider.gameObject.GetComponent<NoteSegment>();

                if (segment == null ||
                    segment.HasBeenHit)
                    return;

                segment.CanBeHit = true;
            }
            else 
                note.CanBeHit = true;
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject == null)
                return;

            Note note = collider.gameObject.GetComponent<Note>();

            if (note == null)
            {
                NoteSegment segment = collider.gameObject.GetComponent<NoteSegment>();

                if (segment == null ||
                    segment.HasBeenHit)
                    return;

                segment.CanBeHit = false;
            }

            note.CanBeHit = false;
        }
    }
}
