using Assets.Scripts.Controls;
using UnityEngine;

namespace Assets.Scripts.TaikoGame.Notes
{
    public class Note : MonoBehaviour
    {
        /// <summary>
        /// The note order
        /// </summary>
        public int OrderIndex { get; private set; }
        /// <summary>
        /// The time when the note was spawned
        /// </summary>
        public float StartTime { get; private set; }
        /// <summary>
        /// The notes color
        /// </summary>
        public float HitTime { get; private set; }
        /// <summary>
        /// The amount a note can be away from the <see cref="_hitPos"/> to be still valid to be hit
        /// </summary>
        public Vector3 HitSize { get; private set; }
        /// <summary>
        /// The time when the note should be hit
        /// </summary>
        public NoteColor Color { get; private set; }
        /// <summary>
        /// The current state, e.g.: Note was hit
        /// </summary>
        public NoteState State { get; private set; }
        /// <summary>
        /// Is the note big?
        /// </summary>
        public bool IsBigNote { get; private set; }

        Vector3 _startPos;
        Vector3 _hitPos;
        Vector3 _destroyPos;

        Vector3 _minHitPos;
        Vector3 _maxHitPos;
        static bool _autoplaySwitchRed;
        static bool _autoplaySwitchBlue;

        /// <summary>
        /// Invoke a note hit
        /// </summary>
        public virtual bool OnHit(bool goodHit)
        {
            if (State != NoteState.Spawned)
                return false;

            State = NoteState.Hit;
            StatisticHandler.Instance.OnNoteHit(goodHit, IsBigNote);
            NoteScroller.Instance.RemoveNoteFromSpawnedList(this);
            Delete();
            return true;
        }

        /// <summary>
        /// Invoke a note miss
        /// </summary>
        public virtual void OnMissed()
        {
            if (State != NoteState.Spawned)
                return;

            State = NoteState.Missed;
            StatisticHandler.Instance.OnNoteMiss();
            NoteScroller.Instance.RemoveNoteFromSpawnedList(this);
        }

        /// <summary>
        /// Invoked when note reaches the end
        /// </summary>
        public virtual void OnReachedEnd()
        {
            NoteScroller.Instance.RemoveNoteFromSpawnedList(this);

            switch (State)
            {
                case NoteState.Spawned:
                case NoteState.Missed:
                    break;

                default:
                    return;
            }

            Delete();
        }

        /// <summary>
        /// Rents a note from the <see cref="NotePool"/>, initializes it and returns it
        /// </summary>
        /// <param name="orderIndex">The note order</param>
        /// <param name="startTime">The time when the note was spawned</param>
        /// <param name="color">The notes color</param>
        /// <param name="hitTime">The time when the note should be hit</param>
        /// <param name="start">Spawn positon</param>
        /// <param name="hit">Position when note should be hit</param>
        /// <param name="destroy">Position when note should be destroyed</param>
        /// <param name="hitSize">The amount a note can be away from the <see cref="_hitPos"/> to be still valid to be hit</param>
        public static Note CreateNew(int orderIndex, float startTime, float hitTime, Vector3 start, Vector3 hit, Vector3 destroy, Vector3 hitSize, 
                                     bool bigNote, Sprite defaultNoteSprite, Sprite defaultNoteOverlaySprite, NoteColor color)
        {
            Note n = NotePool.RentOne();

            if (n.transform.childCount == 0)
            {
                GameObject overlayChild = new GameObject();
                overlayChild.AddComponent<SpriteRenderer>().sprite = defaultNoteOverlaySprite;

                overlayChild.transform.SetParent(n.transform);
                overlayChild.transform.localPosition = Vector3.zero;
            }

            n.OrderIndex = orderIndex;
            n.Color = color;

            n.StartTime = startTime;
            n.HitTime = hitTime;
            n.HitSize = hitSize;

            n._startPos = start;
            n._hitPos = hit;
            n._minHitPos = new Vector3(hit.x - hitSize.x, hit.y, hit.z);
            n._maxHitPos = new Vector3(hit.x + hitSize.x, hit.y, hit.z);
            n._destroyPos = destroy;

            n.State = NoteState.Spawned;

            if (bigNote)
                n.transform.localScale = ActiveTaikoSettings.NoteScaleBig;
            else
                n.transform.localScale = ActiveTaikoSettings.NoteScaleNormal;

            SpriteRenderer sr = n.GetComponent<SpriteRenderer>();

            switch (color)
            {
                default:
                case NoteColor.Red:
                    sr.color = ActiveTaikoSettings.NoteColorRed;
                    break;

                case NoteColor.Blue:
                    sr.color = ActiveTaikoSettings.NoteColorBlue;
                    break;

                case NoteColor.Yellow:
                    sr.color = ActiveTaikoSettings.NoteColorYellow;
                    break;
            }

            sr.sprite = defaultNoteSprite;

            Vector3 pos = start;
            pos.z = -1000f + orderIndex * .01f;
            
            n.transform.position = pos;
            n.gameObject.SetActive(true);
            return n;
        }

        /// <summary>
        /// Sets the note to inactive and returns it to the <see cref="NotePool"/>
        /// </summary>
        public void Delete()
        {
            State = NoteState.Destroyed;
            NotePool.Return(this);
        }

        float GetDistanceByTime(float time, float speed)
        {
            return speed * time;
        }

        void Update()
        {
            //Don't start if our sound has not started
            if (SoundConductor.Instance.PlayState != PlayState.Playing ||
                State == NoteState.Destroyed)
                return;

            Vector3 pos = transform.position;
            float distToMove = GetDistanceByTime((float)(SoundConductor.Instance.CurrentTime - StartTime), ActiveTaikoSettings.NoteSpeed);
            pos.x -= distToMove;

            transform.position = pos = new Vector3(_startPos.x - distToMove, transform.position.y, transform.position.z);

            if (pos.x <= _destroyPos.x)
            {
                OnReachedEnd();
            }
            else if (pos.x < _minHitPos.x)
            {
                OnMissed();
            }
            else if (pos.x >= _minHitPos.x &&
                     pos.x <= _maxHitPos.x &&
                     NoteScroller.Instance.CanNoteBeHit(this))
            {
                if (ActiveTaikoSettings.IsAutoplayActive)
                {
                    // Only hit when we are on hit or already passed it
                    if (pos.x <= _hitPos.x)
                    {
                        OnHit(true);

                        switch(Color)
                        {
                            case NoteColor.Blue:
                                if (IsBigNote)
                                {
                                    Hotkeys.GetKey(HotkeyType.TaikoOuterLeft)
                                           .InvokeKeyDown();
                                    Hotkeys.GetKey(HotkeyType.TaikoOuterRight)
                                           .InvokeKeyDown();
                                }
                                else
                                {
                                    if (_autoplaySwitchBlue)
                                        Hotkeys.GetKey(HotkeyType.TaikoOuterLeft)
                                               .InvokeKeyDown();
                                    else
                                        Hotkeys.GetKey(HotkeyType.TaikoOuterRight)
                                               .InvokeKeyDown();

                                    _autoplaySwitchBlue = !_autoplaySwitchBlue;
                                }
                                break;

                            case NoteColor.Red:
                                if (IsBigNote)
                                {
                                    Hotkeys.GetKey(HotkeyType.TaikoInnerLeft)
                                           .InvokeKeyDown();
                                    Hotkeys.GetKey(HotkeyType.TaikoInnerRight)
                                           .InvokeKeyDown();
                                }
                                else
                                {
                                    if (_autoplaySwitchRed)
                                        Hotkeys.GetKey(HotkeyType.TaikoInnerLeft)
                                               .InvokeKeyDown();
                                    else
                                        Hotkeys.GetKey(HotkeyType.TaikoInnerRight)
                                               .InvokeKeyDown();

                                    _autoplaySwitchRed = !_autoplaySwitchRed;
                                }
                                break;

                            case NoteColor.Yellow:
                                if (Random.Range(1, 3) == 1)
                                    goto case NoteColor.Blue;
                                else
                                    goto case NoteColor.Red;

                        }
                    }
                }
                else
                {
                    int hitCount = 0;
                    switch (Color)
                    {
                        case NoteColor.Blue:
                            if (Hotkeys.CheckKey(HotkeyType.TaikoOuterLeft))
                                hitCount++;
                            if (Hotkeys.CheckKey(HotkeyType.TaikoOuterRight))
                                hitCount++;
                            break;

                        case NoteColor.Red:
                            if (Hotkeys.CheckKey(HotkeyType.TaikoInnerLeft))
                                hitCount++;
                            if (Hotkeys.CheckKey(HotkeyType.TaikoInnerRight))
                                hitCount++;
                            break;

                        case NoteColor.Yellow:
                            if (Hotkeys.CheckKey(HotkeyType.TaikoOuterLeft))
                                hitCount++;
                            if (Hotkeys.CheckKey(HotkeyType.TaikoOuterRight))
                                hitCount++;
                            if (Hotkeys.CheckKey(HotkeyType.TaikoInnerLeft))
                                hitCount++;
                            if (Hotkeys.CheckKey(HotkeyType.TaikoInnerRight))
                                hitCount++;
                            break;
                    }

                    if (IsBigNote)
                    {
                        // Good hit
                        if (hitCount >= 2)
                        {
                            OnHit(true);
                        }
                        // Bad hit
                        else
                        {
                            OnHit(false);
                        }
                    }
                    // Good hit
                    else if (hitCount > 1)
                    {
                        OnHit(true);
                    }
                }
            }
        }
    }
}
