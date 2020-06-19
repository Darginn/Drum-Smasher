using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DSServerCommon;
using System.Security.AccessControl;
using System.Text;
using UnityEngine;

namespace DrumSmasher.Charts
{
    public static class ChartFile
    {
        private const string SECTION_SETTINGS = "[settings]";
        private const string SECTION_NOTES = "[notes]";
        private const string SECTION_END = @"[/\]";
        private const string COMMENT = "//";
        private static readonly List<string> _pathFixList = new List<string>()
        {
            "<", ">",
            "|",
            "?",
            '"'.ToString()
        };

        public static string FixPath(string path)
        {
            string p = string.Copy(path);

            foreach (string str in _pathFixList)
                p = p.Replace(str, "");

            return p;
        }

        public static void Save(Chart chart, DirectoryInfo saveLocation)
        {
            string artist = FixPath(chart.Artist);
            string title = FixPath(chart.Title);
            string creator = FixPath(chart.Creator);
            string difficulty = FixPath(chart.Difficulty);

            if (!saveLocation.Exists)
            {
                Logger.Log("Chart Folder doesn't exist, creating new one:", LogLevel.Warning);
                saveLocation.Create();
                Logger.Log($"Directory {saveLocation.FullName} created successfully");
            }

            string chartFileSW = Path.Combine(saveLocation.FullName, $"{artist} - {title} ({creator}) [{difficulty}]" + ".chart");

            FileInfo file = new FileInfo(chartFileSW);
            Logger.Log("Saving chart to " + file.FullName);

            if (!file.Directory.Exists)
                file.Directory.Create();
            else if (file.Exists)
                file.Delete();
            
            using (StreamWriter swriter = new StreamWriter(chartFileSW))
            {
                swriter.WriteLine(SECTION_SETTINGS);
                PropertyInfo[] props = chart.GetType().GetProperties();

                string line;
                foreach (PropertyInfo prop in props)
                {
                    if (!prop.CanRead || prop.Name.Equals("notes", StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    line = prop.Name + "=";

                    if (prop.PropertyType == typeof(Int16) || prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Int64) ||
                        prop.PropertyType == typeof(UInt16) || prop.PropertyType == typeof(UInt32) || prop.PropertyType == typeof(UInt64) ||
                        prop.PropertyType == typeof(string) || prop.PropertyType == typeof(Single) || prop.PropertyType == typeof(Decimal) ||
                        prop.PropertyType == typeof(Double))
                    {
                        object val = prop.GetValue(chart);

                        if (val != null)
                            line += val.ToString();
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        object val = prop.GetValue(chart);

                        if (val != null)
                        {
                            bool b = (bool)val;
                            line += ToString(b);
                        }
                    }
                    else if (prop.PropertyType.IsAssignableFrom(typeof(Enum)))
                    {
                        object val = prop.GetValue(chart);

                        if (val != null)
                        {
                            Enum e = (Enum)val;

                            line += e.ToString();
                        }
                    }
                    else
                    {
                        Logger.Log("Not supported propertyType: " + prop.PropertyType, LogLevel.Warning);
                    }

                    swriter.WriteLine(line);
                }
                swriter.Flush();
                swriter.WriteLine(SECTION_END);

                swriter.WriteLine(SECTION_NOTES);
                foreach (ChartNote note in chart.Notes)
                {
                    line = ToString(note.BigNote) + "=" + note.Time.TotalMilliseconds.ToString() + "=" + note.Color.ToString();
                    swriter.WriteLine(line);
                }
                swriter.Flush();
                swriter.WriteLine(SECTION_END);
            }

            Logger.Log("Finished save process for " + chartFileSW);
        }

        public static Chart Load(string file)
        {
            if (!File.Exists(file))
            {
                Logger.Log("Failed to load chart file " + file, LogLevel.Warning);
                return null;
            }

            Chart c = new Chart();
            c.Notes = new List<ChartNote>();
            PropertyInfo[] props = typeof(Chart).GetProperties();
            using (StreamReader sreader = new StreamReader(file))
            {
                try
                {
                    bool settings = false;
                    bool notes = false;
                    string line;
                    string[] lineSplit;
                    ChartNote cn;
                    while(!sreader.EndOfStream)
                    {
                        line = sreader.ReadLine();

                        if (line.StartsWith(SECTION_SETTINGS))
                        {
                            settings = true;
                            continue;
                        }
                        else if (line.StartsWith(SECTION_NOTES))
                        {
                            notes = true;
                            continue;
                        }
                        else if (line.StartsWith(SECTION_END))
                        {
                            settings = false;
                            notes = false;
                            continue;
                        }
                        else if (line.StartsWith(COMMENT))
                            continue;
                        else if (line.Contains(COMMENT))
                        {
                            int index = line.IndexOf(COMMENT);
                            line = line.Remove(index, line.Length - index);
                        }

                        if (settings)
                        {
                            lineSplit = line.Split('=');
                            if (lineSplit == null || lineSplit.Length == 1)
                                continue;

                            PropertyInfo prop = props.FirstOrDefault(p => p.Name.Equals(lineSplit[0]));

                            if (prop == null || prop.Equals(default(PropertyInfo)))
                                continue;

                            if (prop.PropertyType == typeof(Int16) && short.TryParse(lineSplit[1], out short s))
                                prop.SetValue(c, s);
                            else if (prop.PropertyType == typeof(Int32) && int.TryParse(lineSplit[1], out int i))
                                prop.SetValue(c, i);
                            else if (prop.PropertyType == typeof(Int64) && long.TryParse(lineSplit[1], out long l))
                                prop.SetValue(c, l);
                            else if (prop.PropertyType == typeof(UInt16) && ushort.TryParse(lineSplit[1], out ushort us))
                                prop.SetValue(c, us);
                            else if (prop.PropertyType == typeof(UInt32) && uint.TryParse(lineSplit[1], out uint ui))
                                prop.SetValue(c, ui);
                            else if (prop.PropertyType == typeof(UInt64) && ulong.TryParse(lineSplit[1], out ulong ul))
                                prop.SetValue(c, ul);
                            else if (prop.PropertyType == typeof(Single) && float.TryParse(lineSplit[1], out float f))
                                prop.SetValue(c, f);
                            else if (prop.PropertyType == typeof(Double) && double.TryParse(lineSplit[1], out double d))
                                prop.SetValue(c, d);
                            else if (prop.PropertyType == typeof(Decimal) && decimal.TryParse(lineSplit[1], out decimal m))
                                prop.SetValue(c, m);
                            else if (prop.PropertyType == typeof(bool))
                                prop.SetValue(c, ToBool(lineSplit[1]));
                            else if (prop.PropertyType == typeof(string))
                                prop.SetValue(c, lineSplit[1]);
                            else if (prop.PropertyType.IsAssignableFrom(typeof(Enum)))
                                prop.SetValue(c, Enum.Parse(prop.PropertyType, lineSplit[1]));
                            else
                                Logger.Log("Unsupported property type " + prop.PropertyType.ToString(), LogLevel.Warning);
                        }
                        else if (notes)
                        {
                            lineSplit = line.Split('=');
                            if (lineSplit == null || lineSplit.Length <= 2)
                                continue;

                            if (long.TryParse(lineSplit[1], out long ms) && short.TryParse(lineSplit[2], out short color))
                            {
                                cn = new ChartNote(TimeSpan.FromMilliseconds(ms), ToBool(lineSplit[0]), color);
                                c.Notes.Add(cn);
                            }
                            else
                                Logger.Log("Could not parse time " + lineSplit[1], LogLevel.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception at reading chart " + file + Environment.NewLine + ex.ToString(), LogLevel.Error);
                }
            }

            return c;
        }

        public static Chart ConvertOsuFile(string file)
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
                while(!sreader.EndOfStream)
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

            Chart ch = new Chart();
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

        public class OsuFile
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

        public class OsuSection : IEquatable<OsuSection>
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

        public class OsuProperty : IEquatable<OsuProperty>
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
        
        private static string ToString(bool val)
        {
            return val ? "1" : "0";
        }
        private static bool ToBool(string val)
        {
            switch(val)
            {
                default:
                case "0":
                    return false;
                case "1":
                    return true;
            }
        }
    }
}
