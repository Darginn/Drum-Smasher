﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Game
{
    public class HitBox : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject == null)
                return;

            Note note = collider.gameObject.GetComponent<Note>();

            if (note == null)
                return;

            note.CanBeHit = true;
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject == null)
                return;

            Note note = collider.gameObject.GetComponent<Note>();

            if (note == null)
                return;

            note.CanBeHit = false;
        }
    }
}
