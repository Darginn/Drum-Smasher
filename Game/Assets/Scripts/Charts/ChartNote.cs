using System;
using System.Collections.Generic;
using System.Text;

namespace DrumSmasher.Assets.Scripts.Charts
{
    public class ChartNote : IEquatable<ChartNote>
    {
        public TimeSpan Time { get; set; }
        public bool BigNote { get; set; }
        public short Color { get; set; }
        public bool IsSlider { get; set; }
        public TimeSpan SliderDuration { get; set; }

        public ChartNote(TimeSpan time, bool bigNote, short color)
        {
            Time = time;
            BigNote = bigNote;
            Color = color;
        }

        public ChartNote()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChartNote);
        }

        public bool Equals(ChartNote other)
        {
            return other != null &&
                   Time.Equals(other.Time) &&
                   BigNote == other.BigNote &&
                   Color == other.Color;
        }

        public override int GetHashCode()
        {
            var hashCode = -724250635;
            hashCode = hashCode * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(Time);
            hashCode = hashCode * -1521134295 + BigNote.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ChartNote note1, ChartNote note2)
        {
            return EqualityComparer<ChartNote>.Default.Equals(note1, note2);
        }

        public static bool operator !=(ChartNote note1, ChartNote note2)
        {
            return !(note1 == note2);
        }
    }
}
