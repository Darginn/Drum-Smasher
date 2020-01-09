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
        public bool AutoPlay;
        public double AutoPlayClickDelay;
        public Notes.NoteScroller Scroller;

        private SpriteRenderer _spriteRenderer;
        private DateTime _clickEnd;

        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            try
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
                else
                {
                    if (_clickEnd <= DateTime.Now)
                    {
                        Logger.Log($"Autoplay note up {DateTime.Now.Second}:{DateTime.Now.Millisecond}");
                        _spriteRenderer.sprite = DefaultImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.ERROR);
            }
        }

        private void KeyPress()
        {
            if (Scroller != null)
            {
                Scroller.Tracker.OnKeyHit(KeyToPress);
            }
        }

        public void SimulateMouseKey(double delay)
        {
            try
            {
                Logger.Log($"Autoplay note down {DateTime.Now.Second}:{DateTime.Now.Millisecond}");
                _spriteRenderer.sprite = PressedImage;
                _clickEnd = DateTime.Now.AddMilliseconds(delay);
                KeyPress();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.ERROR);
            }
        }
    }
}