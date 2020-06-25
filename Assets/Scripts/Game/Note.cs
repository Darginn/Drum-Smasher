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

        public bool CanBeHit;
        public float HitRange;
        public StatisticHandler StatisticHandler;

        public SpriteRenderer Renderer;
        public SpriteRenderer OverlayRenderer;

        public bool IgnoreColor;

        public SoundConductor Conductor { get; set; }
        public TaikoDrumHotKey Key1Controller { get; set; }
        public TaikoDrumHotKey Key2Controller { get; set; }
        public TaikoDrumHotKey Key3Controller { get; set; }
        public TaikoDrumHotKey Key4Controller { get; set; }

        private NoteType _noteType;
        private NoteColor _noteColor;
        
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _hitCirclePosition;
        [SerializeField] private Vector3 _endPosition;
        [SerializeField] private Vector3 _noteSmallScale;
        [SerializeField] private Vector3 _noteBigScale;
        [SerializeField] private Color _noteColorRed;
        [SerializeField] private Color _noteColorBlue;

        private static bool _autoPlaySwitch;

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

            if (CanBeHit)
                CheckForNoteHit();
            
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

            _noteColor = color;
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

                    if (_noteType == NoteType.Big)
                    {
                        OnNoteHit(HitType.GoodHit);

                        switch(_noteColor)
                        {
                            case NoteColor.Blue:
                                Key1Controller.OnKeyDown();
                                Key4Controller.OnKeyDown();
                                break;
                            case NoteColor.Red:
                                Key2Controller.OnKeyDown();
                                Key3Controller.OnKeyDown();
                                break;
                        }
                    }
                    else
                    {
                        if (_autoPlaySwitch)
                        {
                            OnNoteHit(HitType.GoodHit);
                            switch(_noteColor)
                            {
                                case NoteColor.Blue:
                                    Key1Controller.OnKeyDown();
                                    break;
                                case NoteColor.Red:
                                    Key2Controller.OnKeyDown();
                                    break;
                            }
                        }
                        else
                        {
                            OnNoteHit(HitType.GoodHit);
                            switch (_noteColor)
                            {
                                case NoteColor.Blue:
                                    Key4Controller.OnKeyDown();
                                    break;
                                case NoteColor.Red:
                                    Key3Controller.OnKeyDown();
                                    break;
                            }
                        }

                        _autoPlaySwitch = !_autoPlaySwitch;
                    }
                }

                return;
            }

            //We can hit the note
            if (CanBeHit)
            {
                //Check for player input
                OnNoteHit(GetHitType());
            }
        }

        private short CheckForHit()
        {
            short value = 0;

            if (!IgnoreColor)
            {
                TaikoDrumHotKey hotkey1;
                TaikoDrumHotKey hotkey2;

                switch (_noteColor)
                {
                    default:
                    case NoteColor.Blue:
                        hotkey1 = Key1Controller;
                        hotkey2 = Key2Controller;
                        break;

                    case NoteColor.Red:
                        hotkey1 = Key3Controller;
                        hotkey2 = Key4Controller;
                        break;
                }


                if (hotkey1.IsKeyDown && hotkey1.HoldingSince == 0f)
                    value++;
                if (hotkey2.IsKeyDown && hotkey2.HoldingSince == 0f)
                    value++;
            }
            else
            {
                if (Key1Controller.IsKeyDown && Key1Controller.HoldingSince == 0f)
                    value++;
                if (Key2Controller.IsKeyDown && Key2Controller.HoldingSince == 0f)
                    value++;
                if (Key3Controller.IsKeyDown && Key3Controller.HoldingSince == 0f)
                    value++;
                if (Key4Controller.IsKeyDown && Key4Controller.HoldingSince == 0f)
                    value++;
            }

            return value;
        }

        private HitType GetHitType()
        {
            int hits = CheckForHit();

            switch (hits)
            {
                case 0:
                    return HitType.Miss;
                case 1:
                    return HitType.BadHit;
                default:
                case 2:
                    return HitType.GoodHit;
            }
        }

        private void OnNoteHit(HitType hitType)
        {
            bool bignote = transform.localScale == _noteBigScale;

            switch(hitType)
            {
                case HitType.BadHit:
                    Conductor.PlayHitSound();
                    Destroy(gameObject);

                    StatisticHandler.OnNoteHit(HitType.BadHit, bignote);

                    return;

                case HitType.GoodHit:
                    Conductor.PlayHitSound();
                    Destroy(gameObject);
                    StatisticHandler.OnNoteHit(HitType.GoodHit, bignote);
                    return;

                case HitType.Miss:
                    StatisticHandler.OnNoteHit(HitType.Miss, bignote);
                    return;

                default:
                case HitType.None:
                    return;
            }

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
