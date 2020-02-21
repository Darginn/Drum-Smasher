using DrumSmasher.GameInput;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher.Notes
{
    public class Note : MonoBehaviour
    {
        public TimeSpan StartTime;
        public double Speed;
        public bool BigNote;

        public GameObject HitCircle;
        public GameObject EndLine;

        public AudioSource HitSound;

        public double HitWindowRangeX;
        public double HitWindowMinX
        {
            get
            {
                return HitCircle.transform.position.x - HitWindowRangeX;
            }
        }
        public double HitWindowMaxX
        {
            get
            {
                return HitCircle.transform.position.x + HitWindowRangeX;
            }
        }

        public bool Active;
        public bool Paused;
        public bool AutoPlay;
        public double AutoPlayDelayMS;
        public NoteTracker Tracker;
        public ButtonController Key1;
        public ButtonController Key2;

        public GameObject RingOverlay;

        public bool Hitted;
        public bool Missed;

        private static bool _autoPlayNext;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!Active || Paused)
                return;

            //check if we missed
            if (!Missed && HitWindowMinX > transform.position.x)
                NoteMiss();

            //check for hit
            if (!Hitted && !Missed)
            {
                if (AutoPlay)
                {
                    if (transform.position.x < HitCircle.transform.position.x)
                    {
                        Key1.AutoPlay = true;
                        Key2.AutoPlay = true;

                        if (BigNote)
                        {
                            Key1.SimulateMouseKey(AutoPlayDelayMS);
                            Key2.SimulateMouseKey(AutoPlayDelayMS);

                            NoteHit(true, true);
                        }
                        else
                        {
                            if (_autoPlayNext)
                            {
                                Key1.SimulateMouseKey(AutoPlayDelayMS);
                                NoteHit(true, false);
                                _autoPlayNext = false;
                            }
                            else
                            {
                                Key2.SimulateMouseKey(AutoPlayDelayMS);
                                NoteHit(false, true);
                                _autoPlayNext = true;
                            }
                        }
                    }
                }
                else
                {
                    if (transform.position.x <= HitCircle.transform.position.x + HitCircle.transform.localScale.x &&
                        transform.position.x >= HitCircle.transform.position.x - HitCircle.transform.localScale.x)
                    {
                        Key1.AutoPlay = false;
                        Key2.AutoPlay = false;

                        bool key = Input.GetKeyDown(Key1.KeyToPress);
                        bool key2 = Input.GetKeyDown(Key2.KeyToPress);

                        if (BigNote)
                        {
                            if (key && key2)
                                NoteHit(true, true);
                            else if (key )
                                NoteHit(true, false);
                            else if (key2)
                                NoteHit(false, true);
                        }
                        else if (key || key2)
                            NoteHit(true, false);
                    }
                }
            }

            //check if we should delete
            if (transform.position.x <= EndLine.transform.position.x)
            {
                Active = false;
                Destroy(gameObject);
                return;
            }

            MoveForward();
        }

        /// <summary>
        /// Triggers when we hit the note
        /// </summary>
        private void NoteHit(bool key1, bool key2)
        {
            Active = false;

            HitSound.Play();

            NoteTracker.HitType type = BigNote ? (key1 && key2 ? NoteTracker.HitType.GoodHit : NoteTracker.HitType.BadHit) : NoteTracker.HitType.GoodHit;

            Tracker.NoteHit(type, BigNote, (key1 ? new KeyCode?(Key1.KeyToPress) : null), (key2 ? new KeyCode?(Key2.KeyToPress) : null));

            Destroy(gameObject);
        }

        /// <summary>
        /// Triggers when we miss the note
        /// </summary>
        private void NoteMiss()
        {
            Missed = true;
            Tracker.NoteHit(NoteTracker.HitType.Miss);
        }


        /// <summary>
        /// Checks if note is in hit window
        /// </summary>
        /// <returns></returns>
        private bool IsInHitWindow()
        {
            if (HitWindowMinX <= transform.position.x &&
                HitWindowMaxX >= transform.position.x)
                return true;

            return false;
        }
        
        /// <summary>
        /// Moves forward
        /// </summary>
        private void MoveForward()
        {
            transform.position = new Vector3(transform.position.x - Time.deltaTime * (float)Speed, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Moves backward
        /// </summary>
        private void MoveBackwards()
        {
            transform.position = new Vector3(transform.position.x + Time.deltaTime * (float)Speed, transform.position.y, transform.position.z);
        }
    }
}
