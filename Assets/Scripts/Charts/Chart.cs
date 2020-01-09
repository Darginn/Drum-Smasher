using System;
using System.Collections.Generic;
using System.Text;

namespace DrumSmasher.Charts
{
    public class Chart : IEquatable<Chart>
    {
        public int ID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public short Year { get; set; }
        public string Difficulty { get; set; }
        public string Creator { get; set; }
        public string Tags { get; set; }
        public short Genre { get; set; }
        public string Source { get; set; }

        public long Offset { get; set; }
        public long PreviewStart { get; set; }
        public long PreviewEnd { get; set; }


        public float BPM { get; set; }

        public string SoundFile { get; set; }

        public List<ChartNote> Notes { get; set; }

        public Chart(int id, string artist, string title, string album, short year, 
                     string creator, string tags, long offset, long previewStart, 
                     long previewEnd, float bpm, string soundFile, short genre, List<ChartNote> notes = null)
        {
            ID = id;
            Artist = artist;
            Title = title;
            Album = album;
            Year = year;
            Creator = creator;
            Tags = tags;
            Offset = offset;
            PreviewStart = previewStart;
            PreviewEnd = previewEnd;
            BPM = bpm;
            SoundFile = soundFile;
            Genre = genre;

            if (notes == null)
                Notes = new List<ChartNote>();
            else
                Notes = notes;
        }

        public Chart()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Chart);
        }

        public bool Equals(Chart other)
        {
            return other != null &&
                   ID == other.ID &&
                   Artist == other.Artist &&
                   Title == other.Title;
        }

        public override int GetHashCode()
        {
            var hashCode = -219172681;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Artist);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            return hashCode;
        }

        public static bool operator ==(Chart chart1, Chart chart2)
        {
            return EqualityComparer<Chart>.Default.Equals(chart1, chart2);
        }

        public static bool operator !=(Chart chart1, Chart chart2)
        {
            return !(chart1 == chart2);
        }
    }
}
