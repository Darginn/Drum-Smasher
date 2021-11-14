using Assets.Scripts.GameInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Notes
{
    public class Note : MonoBehaviour
    {
        public int NoteId;
        public float Speed = float.MaxValue;
        public float StartTime = float.MaxValue;

        public bool CanBeHit;
        public float HitRange;
        public StatisticHandler StatisticHandler;
        public NoteScroller NoteScroller;

        public SpriteRenderer Renderer;
        public SpriteRenderer OverlayRenderer;

        public bool IgnoreColor;

        public bool HasBeenHit { get; private set; }
        public float SliderDuration { get; private set; }
        public bool IsSlider => _noteType == NoteType.BigLong || _noteType == NoteType.SmallLong;

        public SoundConductor Conductor { get; set; }
        public TaikoDrumHotKey Key1 { get; set; }
        public TaikoDrumHotKey Key2 { get; set; }
        public TaikoDrumHotKey Key3 { get; set; }
        public TaikoDrumHotKey Key4 { get; set; }

        static bool _autoPlaySwitch;
        static bool _lastHitRed;
        static bool _lastHitBlue;

        bool _destroyThis = false;
        NoteType _noteType;
        NoteColor _noteColor;
        bool _missed;
        bool _canBeHitWasTrue;

        [SerializeField] Vector3 _startPosition;
        [SerializeField] Vector3 _hitCirclePosition;
        [SerializeField] Vector3 _endPosition;
        [SerializeField] Vector3 _noteSmallScale;
        [SerializeField] Vector3 _noteBigScale;
        [SerializeField] Color _noteColorRed;
        [SerializeField] Color _noteColorBlue;
        [SerializeField] Color _noteColorYellow;

        #region Slider
        [SerializeField] GameObject _prefabSegment;
        [SerializeField] GameObject _prefabSegmentEnd;
        bool _dontDestroySlider;
        int _sliderHitValue;
        #endregion


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
            if (!_dontDestroySlider && (_destroyThis || gameObject.transform.position.x < _endPosition.x))
            {
                Destroy(gameObject);
                return;
            }

            CheckForHit();
        }

        public void ConvertToSlider(float sliderDuration, float bpm)
        {
            SliderDuration = sliderDuration;
            _dontDestroySlider = true;
            SetNoteType(_noteType == NoteType.Big ? NoteType.BigLong : NoteType.SmallLong, NoteColor.Yellow);

            int segments = 6;

            if (bpm < 150)
                segments = 16;
            else if (bpm < 200)
                segments = 12;
            else if (bpm < 250)
                segments = 8;

            float length = GetDistanceByTime(sliderDuration, Speed);
            float segmentLength = length / segments;
            float scoreCap = (float)StatisticHandler.CalculateSliderScoreCap(Conductor.Length, NoteScroller.Instance.CurrentChart.Notes.Count, sliderDuration, bpm);
            _sliderHitValue = (int)(scoreCap / segments);

            //Instantiate slider middle
            for (int i = 1; i < segments; i++)
                _ = CreateSegment(_prefabSegment, segmentLength * i);

            //Instantiate slider end
            NoteSegment nseg = CreateSegment(_prefabSegmentEnd, segmentLength * segments);
            nseg.IsSliderEnd = true;
            nseg.ChartEndX = _endPosition.x;
            //Create filler object between object 0 and 1
            nseg = CreateSegment(_prefabSegment, 0);
            nseg.enabled = false;
            nseg.GetComponent<BoxCollider2D>().enabled = false;
            nseg.transform.localScale = new Vector3(nseg.transform.localScale.x / 2f, nseg.transform.localScale.y, nseg.transform.localScale.z);
            nseg.transform.localPosition = new Vector3(nseg.transform.localScale.x / 2f, nseg.transform.localPosition.y, nseg.transform.localPosition.z);


            NoteSegment CreateSegment(GameObject prefab, float x)
            {
                GameObject seg = Instantiate(prefab, transform);
                NoteSegment ns = seg.GetComponent<NoteSegment>();

                seg.transform.localPosition = new Vector3(x, 0f, -0.5f);
                seg.transform.localScale = new Vector3(segmentLength, seg.transform.localScale.y, seg.transform.localScale.z);

                ns.HitValue = _sliderHitValue;
                ns.StatisticHandler = StatisticHandler;
                ns.BigNote = _noteType == NoteType.Big || _noteType == NoteType.BigLong;
                ns.SetColor(_noteColorYellow);

                ns.Key1 = Key1;
                ns.Key2 = Key2;
                ns.Key3 = Key3;
                ns.Key4 = Key4;

                return ns;
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

                case NoteType.SmallLong:
                    gameObject.transform.localScale = _noteSmallScale;
                    break;

                case NoteType.BigLong:
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

                case NoteColor.Yellow:
                    gameObject.GetComponent<SpriteRenderer>().color = _noteColorYellow;
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

        void CheckForHit()
        {

            if (_missed)
                return;
            else if (!CanBeHit && !HasBeenHit && _canBeHitWasTrue)
            {
                _missed = true;
                StatisticHandler.OnNoteHit(NoteHitType.Miss, _noteType == NoteType.Big ? true : false);
                return;
            }
            else if (CanBeHit)
            {
                if (!_canBeHitWasTrue)
                    _canBeHitWasTrue = true;

                TaikoDrumHotKey hotkey1 = null;
                TaikoDrumHotKey hotkey2 = null;
                int hitValue = 0;
                int hv = 0;

                switch (_noteColor)
                {
                    case NoteColor.Yellow:
                        switch (_noteType)
                        {
                            case NoteType.BigLong:
                            case NoteType.Big:
                                if (Key1.IsKeyDown && Key1.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key1;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key1;
                                }
                                if (Key2.IsKeyDown && Key2.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key2;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key2;
                                }
                                if (Key3.IsKeyDown && Key3.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key3;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key3;
                                }
                                if (Key4.IsKeyDown && Key4.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key4;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key4;
                                }

                                if (hv > 2)
                                    hitValue = 2;
                                else
                                    hitValue = hv;
                                break;

                            case NoteType.SmallLong:
                            case NoteType.Small:
                                if (Key1.IsKeyDown && Key1.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key1;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key1;
                                }
                                if (Key2.IsKeyDown && Key2.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key2;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key2;
                                }
                                if (Key3.IsKeyDown && Key3.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key3;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key3;
                                }
                                if (Key4.IsKeyDown && Key4.HoldingSince == 0f)
                                {
                                    hv++;

                                    if (hotkey1 == null)
                                        hotkey1 = Key4;
                                    else if (hotkey2 == null)
                                        hotkey2 = Key4;
                                }

                                if (hv >= 1)
                                    hitValue = 2;
                                else
                                    hitValue = hv;
                                break;
                        }
                        break;

                    case NoteColor.Red:
                        switch (_noteType)
                        {
                            case NoteType.BigLong:
                            case NoteType.Big:
                                if (Key2.IsKeyDown && Key2.HoldingSince == 0f)
                                    hitValue++;

                                if (Key3.IsKeyDown && Key3.HoldingSince == 0f)
                                    hitValue++;

                                hotkey1 = Key2;
                                hotkey2 = Key3;
                                break;

                            case NoteType.SmallLong:
                            case NoteType.Small:
                                if ((Key2.IsKeyDown && Key2.HoldingSince == 0f) ||
                                    (Key3.IsKeyDown && Key3.HoldingSince == 0f))
                                    hitValue += 2;

                                hotkey1 = Key2;
                                hotkey2 = Key3;
                                break;
                        }
                        break;

                    case NoteColor.Blue:
                        switch (_noteType)
                        {
                            case NoteType.BigLong:
                            case NoteType.Big:

                                if (Key1.IsKeyDown && Key1.HoldingSince == 0f)
                                    hitValue++;

                                if (Key4.IsKeyDown && Key4.HoldingSince == 0f)
                                    hitValue++;

                                hotkey1 = Key1;
                                hotkey2 = Key4;
                                break;

                            case NoteType.SmallLong:
                            case NoteType.Small:
                                if ((Key1.IsKeyDown && Key1.HoldingSince == 0f) ||
                                    (Key4.IsKeyDown && Key4.HoldingSince == 0f))
                                    hitValue += 2;

                                hotkey1 = Key1;
                                hotkey2 = Key4;
                                break;
                        }
                        break;
                }

                if (NoteScroller.AutoPlay)
                {
                    TaikoDrumHotKey GetRandomKey()
                    {
                        int rnd = UnityEngine.Random.Range(0, 4);

                        switch (rnd)
                        {
                            default:
                            case 0:
                                return Key1;
                            case 1:
                                return Key2;
                            case 2:
                                return Key3;
                            case 3:
                                return Key4;
                        }
                    }

                    if (transform.position.x <= _hitCirclePosition.x)
                        OnNoteHit(NoteHitType.GoodHit, GetRandomKey(), GetRandomKey());
                }
                else
                {
                    if (hitValue > 0)
                    {
                        switch (hitValue)
                        {
                            default:
                            case 1:
                                OnNoteHit(NoteHitType.BadHit, hotkey1, hotkey2);
                                break;
                            case 2:
                                OnNoteHit(NoteHitType.GoodHit, hotkey1, hotkey2);
                                break;
                        }
                    }
                }
            }
        }

        void OnNoteHit(NoteHitType hit, TaikoDrumHotKey hotkey1, TaikoDrumHotKey hotKey2)
        {
            if (hit == NoteHitType.Miss ||
                HasBeenHit)
                return;

            HasBeenHit = true;
            
            bool bignote = transform.localScale == _noteBigScale;

            if (NoteScroller.AutoPlay)
            {
                if (_noteType == NoteType.BigLong || _noteType == NoteType.SmallLong)
                    StatisticHandler.OnNoteHit(NoteHitType.GoodHit, bignote, true, _sliderHitValue);
                else
                    StatisticHandler.OnNoteHit(NoteHitType.GoodHit, bignote);

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

                if (!_dontDestroySlider)
                    Destroy(gameObject);

                return;
            }

            switch (_noteColor)
            {
                case NoteColor.Red:
                    if (!NoteScroller.AutoPlay && _lastHitRed)
                        return;

                    _lastHitRed = true;
                    break;

                case NoteColor.Blue:
                    if (!NoteScroller.AutoPlay && _lastHitBlue)
                        return;

                    _lastHitBlue = true;
                    break;
            }

            switch (hit)
            {
                case NoteHitType.BadHit:
                    Conductor.PlayHitSound();

                    if (_noteType == NoteType.BigLong || _noteType == NoteType.SmallLong)
                        StatisticHandler.OnNoteHit(NoteHitType.GoodHit, bignote, true, _sliderHitValue);
                    else
                        StatisticHandler.OnNoteHit(NoteHitType.BadHit, bignote, true, _sliderHitValue);

                    StartCoroutine(ResetNoteHit(_noteColor));
                    return;

                case NoteHitType.GoodHit:
                    Conductor.PlayHitSound();

                    if (_noteType == NoteType.BigLong || _noteType == NoteType.SmallLong)
                        StatisticHandler.OnNoteHit(NoteHitType.GoodHit, bignote, true, _sliderHitValue);
                    else
                        StatisticHandler.OnNoteHit(NoteHitType.GoodHit, bignote, true, _sliderHitValue);

                    StartCoroutine(ResetNoteHit(_noteColor));
                    return;
            }
        }

        IEnumerator ResetNoteHit(NoteColor color)
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

            if (!_dontDestroySlider)
                Destroy(gameObject);
        }

        float GetDistanceByTime(float time, float speed)
        {
            return speed * time;
        }

        float GetTimeByDistance(float distance, float speed)
        {
            return distance / speed;
        }
    }
}
