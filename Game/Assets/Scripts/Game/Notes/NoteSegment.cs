using Assets.Scripts.GameInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Notes
{
    public class NoteSegment : MonoBehaviour
    {
        public bool CanBeHit { get; set; }
        public bool HasBeenHit { get; set; }
        public bool BigNote { get; set; }
        public float AutoplayTriggerX { get; set; }

        public int HitValue { get; set; }
        public bool IsSliderEnd { get; set; }
        public float ChartEndX { get; set; }
        public StatisticHandler StatisticHandler { get; set; }

        public TaikoDrumHotKey Key1 { get; set; }
        public TaikoDrumHotKey Key2 { get; set; }
        public TaikoDrumHotKey Key3 { get; set; }
        public TaikoDrumHotKey Key4 { get; set; }
        public Note Parent { get; set; }

        void Update()
        {
            if (IsSliderEnd && 
                transform.parent.gameObject.transform.position.x < ChartEndX)
            {
                Destroy(transform.parent.gameObject);
                return;
            }

            if (HasBeenHit || !CanBeHit)
                return;

            if (NoteScroller.Instance.AutoPlay)
            {
                if (transform.position.x <= AutoplayTriggerX)
                {
                    Hit();

                    int key = UnityEngine.Random.Range(0, 5);

                    switch (key)
                    {
                        default:
                        case 0:
                            Key1.OnKeyDown();
                            break;

                        case 1:
                            Key2.OnKeyDown();
                            break;

                        case 2:
                            Key3.OnKeyDown();
                            break;

                        case 3:
                            Key4.OnKeyDown();
                            break;
                    }
                }
            }
            else if (Key1.IsKeyDown && Key1.HoldingSince == 0f ||
                     Key2.IsKeyDown && Key2.HoldingSince == 0f ||
                     Key3.IsKeyDown && Key3.HoldingSince == 0f ||
                     Key4.IsKeyDown && Key4.HoldingSince == 0f)
            {
                Hit();
            }
        }

        void Hit()
        {
            HasBeenHit = true;
            StatisticHandler.OnNoteHit(NoteHitType.GoodHit, BigNote, true, HitValue);
        }

        public void SetColor(Color c)
        {
            GetComponentInChildren<SpriteRenderer>().color = c;
        }
    }
}
