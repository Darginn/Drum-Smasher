using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DrumSmasher.Charts
{
    public static class ChartFile
    {
        private const string SECTION_SETTINGS = "[settings]";
        private const string SECTION_NOTES = "[notes]";
        private const string SECTION_END = @"[/\]";
        private const string COMMENT = "//";
        
        public static void Save(string file, Chart chart)
        {
            Logger.Log("Saving chart to " + file, LogLevel.Trace);
            StreamWriter swriter = new StreamWriter(file);
            try
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
                        Logger.Log("Not supported propertyType: " + prop.PropertyType, LogLevel.WARNING);
                    }

                    swriter.WriteLine(line);
                }
                swriter.Flush();
                swriter.WriteLine(SECTION_END);

                swriter.WriteLine(SECTION_NOTES);
                foreach(ChartNote note in chart.Notes)
                {
                    line = ToString(note.BigNote) + "=" + note.Time.TotalMilliseconds.ToString() + "=" + note.Color.ToString();
                    swriter.WriteLine(line);
                }
                swriter.Flush();
                swriter.WriteLine(SECTION_END);
                swriter.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Log("Could not save chart to file " + file + Environment.NewLine + ex.ToString(), LogLevel.ERROR);
                swriter.Dispose();

                if (File.Exists(file))
                    File.Delete(file);
            }
            Logger.Log("Finished save process for " + file, LogLevel.Trace);
        }

        public static Chart Load(string file)
        {
            if (!File.Exists(file))
            {
                Logger.Log("Failed to load chart file " + file, LogLevel.WARNING);
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
                                Logger.Log("Unsupported property type " + prop.PropertyType.ToString(), LogLevel.WARNING);
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
                                Logger.Log("Could not parse time " + lineSplit[1], LogLevel.WARNING);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("Exception at reading chart " + file + Environment.NewLine + ex.ToString(), LogLevel.ERROR);
                }
            }

            return c;
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
