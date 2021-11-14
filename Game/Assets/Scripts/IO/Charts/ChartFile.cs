using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.IO.Charts
{
    public class ChartFile : JsonFile<ChartFile>
    {
        public int ID { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public short Year { get; set; }
        public string Difficulty { get; set; }
        public string Creator { get; set; }
        public string Tags { get; set; }
        public string Genre { get; set; }
        public string Source { get; set; }

        public long Offset { get; set; }
        public long PreviewStart { get; set; }
        public long PreviewEnd { get; set; }

        public int Speed { get; set; }
        public float BPM { get; set; }

        public string SoundFile { get; set; }

        public List<ChartNote> Notes { get; set; }

        static readonly List<string> _pathFixList = new List<string>()
        {
            "<", ">",
            "|",
            "?",
            '"'.ToString()
        };

        public ChartFile(int id, string artist, string title, string album, short year,
                         string creator, string tags, long offset, long previewStart,
                         long previewEnd, float bpm, string soundFile, string genre, List<ChartNote> notes = null)
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

        public ChartFile()
        {

        }

        public static ChartFile ConvertOsuFile(string file)
        {
            FileInfo fI = new FileInfo(file);

            if (!fI.Exists)
                return null;

            OsuFile osuFile = new OsuFile(fI.Name + fI.Extension, fI.Directory.FullName, new Dictionary<string, OsuSection>());

            using (StreamReader sreader = new StreamReader(fI.FullName))
            {
                bool readSection = true;
                bool readProps = false;
                string sectionName = null;
                string line;
                while (!sreader.EndOfStream)
                {
                    line = sreader.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    Begin:
                    if (readSection)
                    {
                        if (line[0].Equals('['))
                        {
                            line = line.Trim('[', ']', ' ');

                            sectionName = line;
                            Logger.Log(sectionName);
                            osuFile.Sections.Add(line, new OsuSection(line, new Dictionary<string, OsuProperty>()));

                            readSection = false;
                            readProps = true;
                        }
                        continue;
                    }

                    if (readProps)
                    {
                        if (line[0].Equals('['))
                        {
                            readSection = true;
                            readProps = false;
                            goto Begin;
                        }

                        OsuProperty prop;
                        switch (sectionName.ToLower())
                        {
                            case "difficulty":
                            case "events":
                            case "editor":
                                continue;

                            case "timingpoints":
                                string[] timingSplit = line.Split(',');
                                int.TryParse(timingSplit[0], out int offset);
                                osuFile.Offset = offset;
                                float.TryParse(timingSplit[1], out float bpm);
                                osuFile.BPM = bpm;
                                readSection = true;
                                readProps = false;
                                continue;

                            case "hitobjects":
                                string[] noteSplit = line.Split(',').Skip(2).ToArray();

                                if (noteSplit == null || noteSplit.Length < 3)
                                    continue;

                                long[] iNoteSplit = new long[noteSplit.Length];
                                for (int i = 0; i < noteSplit.Length; i++)
                                    long.TryParse(noteSplit[i], out iNoteSplit[i]);

                                prop = new OsuProperty("Note." + noteSplit[0], new long[] { iNoteSplit[0], iNoteSplit[1], iNoteSplit[2] });
                                osuFile.Sections[sectionName].Properties.Add(prop.Name, prop);
                                continue;

                            default:
                                break;
                        }

                        string[] propSplit = line.Split(':');

                        if (propSplit == null || propSplit.Length < 2)
                            continue;
                        if (float.TryParse(propSplit[1], out float fval))
                        {
                            prop = new OsuProperty(propSplit[0], fval);
                            osuFile.Sections[sectionName].Properties.Add(prop.Name, prop);
                            continue;
                        }

                        prop = new OsuProperty(propSplit[0], propSplit[1]);
                        osuFile.Sections[sectionName].Properties.Add(prop.Name, prop);

                    }
                }
            }

            OsuSection general = osuFile.Sections["General"];
            OsuSection metadata = osuFile.Sections["Metadata"];

            ChartFile ch = new ChartFile();
            ch.ID = 0;
            ch.Artist = (string)metadata.Properties["Artist"].Value;
            ch.Title = (string)metadata.Properties["Title"].Value;
            ch.Tags = (string)metadata.Properties["Tags"].Value;
            ch.Difficulty = (string)metadata.Properties["Version"].Value;
            ch.Creator = (string)metadata.Properties["Creator"].Value;
            ch.Source = (string)metadata.Properties["Source"].Value;
            ch.PreviewStart = (long)(float)general.Properties["PreviewTime"].Value;
            ch.Offset = osuFile.Offset;
            ch.SoundFile = ((string)general.Properties["AudioFilename"].Value).TrimStart(' ');

            ch.Notes = new List<ChartNote>();
            OsuSection snote = osuFile.Sections["HitObjects"];

            ChartNote cn;
            long[] vals;
            bool bigNote;
            TimeSpan timing;
            short color;
            foreach (var prop in snote.Properties)
            {
                vals = (long[])prop.Value.Value;

                timing = TimeSpan.FromMilliseconds(vals[0]);

                switch (vals[2])
                {
                    default:
                    case 0:
                        bigNote = false;
                        color = 1;
                        break;
                    case 2:
                        bigNote = false;
                        color = 0;
                        break;
                    case 4:
                        bigNote = true;
                        color = 1;
                        break;
                    case 6:
                        bigNote = true;
                        color = 0;
                        break;
                    case 8:
                        bigNote = false;
                        color = 0;
                        break;
                    case 12:
                        bigNote = true;
                        color = 0;
                        break;
                }

                cn = new ChartNote(timing, bigNote, color);

                ch.Notes.Add(cn);
            }

            return ch;
        }

        public static string FixPath(string path)
        {
            string p = string.Copy(path);

            foreach (string str in _pathFixList)
                p = p.Replace(str, "");

            return p;
        }

        #region Osu file
        class OsuFile
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public long Offset { get; set; }
            public float BPM { get; set; }
            public string SoundFile { get; set; }
            public Dictionary<string, OsuSection> Sections { get; set; }

            public OsuFile(string name, string path, Dictionary<string, OsuSection> sections)
            {
                Name = name;
                Path = path;
                Sections = sections;
            }

            public OsuFile()
            {
            }
        }

        class OsuSection : IEquatable<OsuSection>
        {
            public string Name { get; set; }
            public Dictionary<string, OsuProperty> Properties { get; set; }

            public OsuSection(string name, Dictionary<string, OsuProperty> properties)
            {
                Name = name;
                Properties = properties;
            }

            public OsuSection()
            {
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as OsuSection);
            }

            public bool Equals(OsuSection other)
            {
                return other != null &&
                       Name == other.Name;
            }

            public override int GetHashCode()
            {
                return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
            }

            public static bool operator ==(OsuSection section1, OsuSection section2)
            {
                return EqualityComparer<OsuSection>.Default.Equals(section1, section2);
            }

            public static bool operator !=(OsuSection section1, OsuSection section2)
            {
                return !(section1 == section2);
            }
        }

        class OsuProperty : IEquatable<OsuProperty>
        {
            public string Name { get; set; }
            public object Value { get; set; }

            public OsuProperty(string name, object value)
            {
                Name = name;
                Value = value;
            }

            public OsuProperty()
            {
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as OsuProperty);
            }

            public bool Equals(OsuProperty other)
            {
                return other != null &&
                       Name == other.Name &&
                       EqualityComparer<object>.Default.Equals(Value, other.Value);
            }

            public override int GetHashCode()
            {
                var hashCode = -244751520;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Value);
                return hashCode;
            }

            public static bool operator ==(OsuProperty property1, OsuProperty property2)
            {
                return EqualityComparer<OsuProperty>.Default.Equals(property1, property2);
            }

            public static bool operator !=(OsuProperty property1, OsuProperty property2)
            {
                return !(property1 == property2);
            }
        }
        #endregion
    }
}
