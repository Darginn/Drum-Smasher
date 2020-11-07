using DrumSmasher.GameInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Game
{
    public class Note : MonoBehaviour
    {
        public int NoteId;
        public float Speed = float.MaxValue;
        public float StartTime = float.MaxValue;
        public bool AutoPlay = false;

        private static bool _lastHitRed;
        private static bool _lastHitBlue;

        public bool CanBeHit;
        public float HitRange;
        public StatisticHandler StatisticHandler;
        public NoteScroller NoteScroller;

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
        private bool _missed;
        private bool _canBeHitWasTrue;

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

            //We reached our end
            if (_destroyThis || gameObject.transform.position.x < _endPosition.x)
                Destroy(gameObject);

            if (_missed)
                return;
            else if (!CanBeHit && _canBeHitWasTrue)
            {
                _missed = true;
                StatisticHandler.OnNoteHit(HitType.Miss, _noteType == NoteType.Big ? true : false);
                return;
            }

            if (CanBeHit)
            {
                if (!_canBeHitWasTrue)
                    _canBeHitWasTrue = true;

                TaikoDrumHotKey hotkey1 = null;
                TaikoDrumHotKey hotkey2 = null;
                int hitValue = 0;

                switch (_noteColor)
                {
                    case NoteColor.Red:
                        switch (_noteType)
                        {
                            case NoteType.Big:
                                if (Key2Controller.IsKeyDown && Key2Controller.HoldingSince == 0f)
                                    hitValue++;

                                if (Key3Controller.IsKeyDown && Key3Controller.HoldingSince == 0f)
                                    hitValue++;

                                hotkey1 = Key2Controller;
                                hotkey2 = Key3Controller;
                                break;

                            case NoteType.Small:
                                if ((Key2Controller.IsKeyDown && Key2Controller.HoldingSince == 0f) ||
                                    (Key3Controller.IsKeyDown && Key3Controller.HoldingSince == 0f))
                                    hitValue += 2;

                                hotkey1 = Key2Controller;
                                hotkey2 = Key3Controller;
                                break;
                        }
                        break;

                    case NoteColor.Blue:
                        switch (_noteType)
                        {
                            case NoteType.Big:

                                if (Key1Controller.IsKeyDown && Key1Controller.HoldingSince == 0f)
                                    hitValue++;

                                if (Key4Controller.IsKeyDown && Key4Controller.HoldingSince == 0f)
                                    hitValue++;

                                hotkey1 = Key1Controller;
                                hotkey2 = Key4Controller;
                                break;

                            case NoteType.Small:
                                if ((Key1Controller.IsKeyDown && Key1Controller.HoldingSince == 0f) ||
                                    (Key4Controller.IsKeyDown && Key4Controller.HoldingSince == 0f))
                                    hitValue += 2;

                                hotkey1 = Key1Controller;
                                hotkey2 = Key4Controller;
                                break;
                        }
                        break;
                }

                if (AutoPlay)
                {
                    if (transform.position.x <= _hitCirclePosition.x)
                        OnNoteHit(HitType.GoodHit, hotkey1, hotkey2);
                }
                else
                {
                    if (hitValue > 0)
                    {
                        switch (hitValue)
                        {
                            default:
                            case 1:
                                OnNoteHit(HitType.BadHit, hotkey1, hotkey2);
                                break;
                            case 2:
                                OnNoteHit(HitType.GoodHit, hotkey1, hotkey2);
                                break;
                        }
                    }
                }
            }
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

        private void OnNoteHit(HitType hit, TaikoDrumHotKey hotkey1, TaikoDrumHotKey hotKey2)
        {
            if (hit == HitType.Miss)
                return;
            
            bool bignote = transform.localScale == _noteBigScale;

            if (AutoPlay)
            {
                StatisticHandler.OnNoteHit(HitType.GoodHit, bignote);

                if (bignote)
                {
                    hotkey1.OnKeyDown();
                    hotKey2.OnKeyDown();
                }
                else
                {
                    if (_autoPlaySwitch)
                        hotKey2.OnKeyDown();
                    else
                        hotkey1.OnKeyDown();
                    
                    _autoPlaySwitch = !_autoPlaySwitch;
                }

                Destroy(gameObject);
                return;
            }

            switch (_noteColor)
            {
                case NoteColor.Red:
                    if (!AutoPlay && _lastHitRed)
                        return;

                    _lastHitRed = true;
                    break;

                case NoteColor.Blue:
                    if (!AutoPlay && _lastHitBlue)
                        return;

                    _lastHitBlue = true;
                    break;
            }

            switch (hit)
            {
                case HitType.BadHit:
                    Conductor.PlayHitSound();
                    StatisticHandler.OnNoteHit(HitType.BadHit, bignote);
                    StartCoroutine(ResetNoteHit(_noteColor));
                    return;

                case HitType.GoodHit:
                    Conductor.PlayHitSound();
                    StatisticHandler.OnNoteHit(HitType.GoodHit, bignote);
                    StartCoroutine(ResetNoteHit(_noteColor));
                    return;
            }
        }

        private IEnumerator ResetNoteHit(NoteColor color)
        {
            yield return new WaitForEndOfFrame();
            switch(color)
            {
                case NoteColor.Red:
                    _lastHitRed = false;
                    break;

                case NoteColor.Blue:
                    _lastHitBlue = false;
                    break;
            }

            Destroy(gameObject);
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
