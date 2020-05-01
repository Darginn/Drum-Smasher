using DrumSmasher.GameInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Game
{
    public class Note : MonoBehaviour
    {
        public float Speed = float.MaxValue;
        public float StartTime = float.MaxValue;
        public bool AutoPlay = false;
        public float HitRange;

        public SoundConductor Conductor { get; set; }
        public ButtonController Key1Controller { get; set; }
        public ButtonController Key2Controller { get; set; }

        private NoteType _noteType { get; set; }
        
        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField]
        private Vector3 _hitCirclePosition;
        [SerializeField]
        private Vector3 _endPosition;
        [SerializeField]
        private Vector3 _noteSmallScale;
        [SerializeField]
        private Vector3 _noteBigScale;
        [SerializeField]
        private Color _noteColorRed;
        [SerializeField]
        private Color _noteColorBlue;

        private static bool _autoPlaySwitch;

        private bool _canBeHit = true;
        private bool _destroyThis = false;

        void Start()
        {
        }

        void Update()
        {
            //Don't play if we haven't started playing the music yet
            //Or haven't passed our start time
            if (Conductor.PlayState != PlayState.Playing ||
                StartTime > Conductor.CurrentTime)
                return;

            //Update our current position based on time
            UpdatePosition();

            if (_canBeHit)
            {
                //We are out of hitrange
                if ((gameObject.transform.position.x < _hitCirclePosition.x - HitRange))
                {
                    _canBeHit = false;
                    return;
                }

                CheckForNoteHit();
            }
            
            //We reached our end
            if (_destroyThis || gameObject.transform.position.x < _endPosition.x)
                Destroy(gameObject);
        }

        public void SetNoteType(NoteType type, NoteColor color)
        {
            switch (type)
            {
                default:
                case NoteType.Small:
                    gameObject.transform.localScale = _noteSmallScale;
                    break;

                case NoteType.Big:
                    gameObject.transform.localScale = _noteBigScale;
                    break;
            }

            _noteType = type;

            switch (color)
            {
                default:
                case NoteColor.Red:
                    gameObject.GetComponent<SpriteRenderer>().color = _noteColorRed;
                    break;

                case NoteColor.Blue:
                    gameObject.GetComponent<SpriteRenderer>().color = _noteColorBlue;
                    break;
            }
        }

        public void UpdatePosition()
        {
            float dist = GetDistanceByTime((float)(Conductor.CurrentTime - StartTime), Speed);
            Vector3 newPos = new Vector3(_startPosition.x - dist, _startPosition.y, _startPosition.z);

            gameObject.transform.position = newPos;
        }

        public void SetDefaultPosition()
        {
            gameObject.transform.position = _startPosition;
        }

        public void SetStartZ(float value)
        {
            _startPosition.z = value;
        }

        private void CheckForNoteHit()
        {
            if (AutoPlay)
            {
                if (gameObject.transform.position.x <= _hitCirclePosition.x)
                {
                    OnNoteHit(HitType.GoodHit);

                    if (_noteType == NoteType.Big)
                    {
                        Key1Controller.TriggerKey();
                        Key2Controller.TriggerKey();
                    }
                    else
                    {
                        if (_autoPlaySwitch)
                            Key2Controller.TriggerKey();
                        else
                            Key1Controller.TriggerKey();

                        _autoPlaySwitch = !_autoPlaySwitch;
                    }
                }

                return;
            }

            //We can hit the note
            if (CanBeHit())
            {

                //Check for player input
                OnNoteHit(GetHitType());
            }
        }

        private HitType GetHitType()
        {
            int hits = 2;

            if (_noteType == NoteType.Big)
            {
                if (!Input.GetKeyDown(Key1Controller.KeyToPress))
                    hits--;
                if (!Input.GetKeyDown(Key2Controller.KeyToPress))
                    hits--;
            }
            else
            {
                if (!Input.GetKeyDown(Key1Controller.KeyToPress) ||
                    !Input.GetKeyDown(Key2Controller.KeyToPress))
                    hits -= 2;
            }

            switch (hits)
            {
                default:
                case 0:
                    return HitType.Miss;
                case 1:
                    return HitType.BadHit;
                case 2:
                    return HitType.GoodHit;
            }
        }

        private bool CanBeHit()
        {
            return (gameObject.transform.position.x > _hitCirclePosition.x - HitRange) &&
                   (gameObject.transform.position.x < _hitCirclePosition.x + HitRange);
        }

        private void OnNoteHit(HitType hitType)
        {
            Conductor.PlayHitSound();
            Destroy(gameObject);

            Logger.Log("Note hit! ", LogLevel.Trace);
        }

        private float GetDistanceByTime(float time, float speed)
        {
            return speed * time;
        }

        private float GetTimeByDistance(float distance, float speed)
        {
            return distance / speed;
        }
    }
}
