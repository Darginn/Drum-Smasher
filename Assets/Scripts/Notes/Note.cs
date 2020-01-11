using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Notes
{
    public class Note : MonoBehaviour
    {
        public bool BigNote;

        public KeyCode Key;
        public KeyCode Key2;
        public GameInput.ButtonController KeyController;
        public GameInput.ButtonController KeyController2;

        public GameObject GoodEffect;
        public GameObject MissEffect;

        public NoteScroller Scroller;
        public GameObject EndLine;
        public GameObject HitCircle;

        public Vector3 StartPosition;
        public DateTime StartTime;

        public bool DefaultNote;

        private bool _missed;
        private bool _autoLeft;
        
        public Note() : base()
        {

        }

        void Start()
        {

        }

        void Update()
        {
            if (Scroller.Paused || DefaultNote)
                return;
            
            if (Scroller.AutoPlay)
            {
                KeyController.AutoPlay = true;
                KeyController2.AutoPlay = true;

                if (transform.position.x < HitCircle.transform.position.x)
                {
                    if (BigNote)
                    {
                        KeyController.SimulateMouseKey(Scroller.AutoPlayDelayMS);
                        KeyController2.SimulateMouseKey(Scroller.AutoPlayDelayMS);
                    }
                    else if (_autoLeft)
                    {
                        KeyController.SimulateMouseKey(Scroller.AutoPlayDelayMS);
                        _autoLeft = false;
                    }
                    else
                    {
                        KeyController2.SimulateMouseKey(Scroller.AutoPlayDelayMS);
                        _autoLeft = true;
                    }
                    OnHit(true);
                    return;
                }
            }
            else
            {
                KeyController.AutoPlay = false;
                KeyController2.AutoPlay = false;
            }

            if (transform.position.x <= HitCircle.transform.position.x + HitCircle.transform.localScale.x &&
                transform.position.x >= HitCircle.transform.position.x - HitCircle.transform.localScale.x)
            {
                bool key = Input.GetKeyDown(Key);
                bool key2 = Input.GetKeyDown(Key2);

                if (BigNote)
                {
                    if (key && key2)
                    {
                        OnHit(true);
                        return;
                    }
                    else if (key || key2)
                    {
                        OnHit(false);
                        return;
                    }
                }
                else if (key || key2)
                {
                    OnHit(true);
                    return;
                }
            }

            //Move
            transform.position = StartPosition - new Vector3(Scroller.NoteSpeed * (float)DateTime.Now.Subtract(StartTime).TotalSeconds * 3f, 0f);

            //Check if we missed
            if (!_missed && transform.position.x < HitCircle.transform.position.x - HitCircle.transform.localScale.x)
            {
                OnMiss();
                _missed = true;
            }
            //Check if we reached the end
            else if (_missed && transform.position.x < EndLine.transform.position.x)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnMiss()
        {
            Scroller.Tracker.Miss();
        }

        private void OnHit(bool goodHit)
        {
            Scroller.Sound.HitSource.Play();
            Scroller.Tracker.Hit(goodHit, BigNote);
            Destroy(gameObject);
        }
    }
}
