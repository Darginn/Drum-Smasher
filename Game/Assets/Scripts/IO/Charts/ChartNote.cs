using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.IO.Charts
{
    public class ChartNote
    {
        public TimeSpan HitTime { get; set; }
        public TimeSpan SliderDuration { get; set; }

        public short Color { get; set; }

        public bool IsSlider { get; set; }
        public bool IsBigNote { get; set; }

        public ChartNote(TimeSpan hitTime, bool isBigNote, short color)
        {
            HitTime = hitTime;
            IsBigNote = isBigNote;
            Color = color;
        }

        public ChartNote()
        {

        }
    }
}
