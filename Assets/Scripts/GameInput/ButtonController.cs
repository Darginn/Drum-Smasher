using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher.GameInput
{
    public class ButtonController : MonoBehaviour
    {
        public Sprite DefaultImage;
        public Sprite PressedImage;
        public KeyCode KeyToPress;
        public Notes.NoteScroller Scroller;
        public bool AutoplayNextKey;

        public bool AutoPlay;

        private SpriteRenderer _spriteRenderer;
        private DateTime _clickEnd;

        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Load(int level)
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!AutoPlay)
            {
                if (Input.GetKeyDown(KeyToPress))
                {
                    _spriteRenderer.sprite = PressedImage;
                    KeyPress();
                }

                if (Input.GetKeyUp(KeyToPress))
                {
                    _spriteRenderer.sprite = DefaultImage;
                }
            }
            else if (_clickEnd <= DateTime.Now)
            {
                _spriteRenderer.sprite = DefaultImage;
                _clickEnd = DateTime.MaxValue;
            }
        }

        private void KeyPress()
        {
            if (Scroller != null)
            {
                Scroller.Tracker.KeyHit(KeyToPress);
            }
        }

        public void SimulateMouseKey(double delay)
        {
            _spriteRenderer.sprite = PressedImage;
            _clickEnd = DateTime.Now.AddMilliseconds(delay);
        }
    }
}