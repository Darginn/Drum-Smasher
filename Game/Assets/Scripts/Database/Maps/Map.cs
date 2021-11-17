using Assets.Scripts.Database.Scores;
using Assets.Scripts.Enums;
using Assets.Scripts.IO.Charts;
using Assets.Scripts.Configs.Bindable;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Configs.GameConfigs;

namespace Assets.Scripts.Database.Maps
{
    public class Map
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// The MD5 Of the file.
        /// </summary>
        [Unique]
        public string Md5Checksum { get; set; }

        /// <summary>
        /// The directory of the map
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The absolute path of the .chart file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The mapset id of the map.
        /// </summary>
        public int MapSetId { get; set; }

        /// <summary>
        /// The id of the map.
        /// </summary>
        public int MapId { get; set; }

        /// <summary>
        /// The song's artist
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// The song's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The difficulty name of the map
        /// </summary>
        public string DifficultyName { get; set; }

        /// <summary>
        ///     The highest rank that the player has gotten on the map.
        /// </summary>
        public Grade HighestRank { get; set; }

        /// <summary>
        /// The last time the user has played the map.
        /// </summary>
        public long LastTimePlayed { get; set; }

        /// <summary>
        /// The creator of the map.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// The absolute path of the map's background.
        /// </summary>
        public string BackgroundPath { get; set; }

        /// <summary>
        /// The absolute path of the map's audio.
        /// </summary>
        public string AudioPath { get; set; }

        /// <summary>
        /// The audio preview time of the map
        /// </summary>
        public long AudioPreviewTime { get; set; }

        /// <summary>
        /// The description of the map
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The source (album/mixtape/etc) of the map
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Tags for the map
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// The genre of the song
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// The most common bpm for the map
        /// </summary>
        public double Bpm { get; set; }

        /// <summary>
        /// The map's length (Time of the last hit object preferably)
        /// </summary>
        public int SongLength { get; set; }

        /// <summary>
        /// The local offset for this map
        /// </summary>
        public int LocalOffset { get; set; }

        /// <summary>
        /// The last time the file was modified
        /// </summary>
        public DateTime LastFileWrite { get; set; }

        /// <summary>
        /// The date the map was added.
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// The count of notes (including finishers, can split if necessary).
        /// </summary>
        public int NoteCount { get; set; }

        /// <summary>
        /// The count of sliders.
        /// </summary>
        public int SliderCount { get; set; }

        /// <summary>
        /// The count of spinners.
        /// </summary>
        public int SpinnerCount { get; set; }

        /// <summary>
        /// The amount of times the map has been played
        /// </summary>
        public int TimesPlayed { get; set; }

        /// <summary>
        /// Returns the notes per second a map has
        /// </summary>
        [Ignore]
        public float NotesPerSecond
        {
            get
            {
                var objectCount = NoteCount + SliderCount + SpinnerCount;
                float nps = Mathf.Clamp(objectCount / (SongLength / 1000), 0, float.MaxValue);

                return nps;
            }
        }

        /// <summary>
        /// Determines if this map is an osu! map.
        /// </summary>
        [Ignore]
        public MapGame Game { get; set; } = MapGame.DrumSmasher;

        /// <summary>
        /// The actual parsed chart file for the map.
        /// </summary>
        [Ignore]
        public ChartFile Chart { get; set; }

        /// <summary>
        /// The mapset the map belongs to.
        /// </summary>
        [Ignore]
        public Mapset Mapset { get; set; }

        /// <summary>
        /// The scores for this map.
        /// </summary>
        [Ignore]
        public Bindable<List<Score>> Scores { get; } = new Bindable<List<Score>>(null);

        /// <summary>
        ///     Responsible for converting a ChartFile object, to a Map object
        ///     a Map object is one that is stored in the db.
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Map FromChart(ChartFile chart, string path, bool skipPathSetting = false)
        {
            var map = new Map
            {
                Artist = chart.Artist,
                Title = chart.Title,
                HighestRank = Grade.None,
                AudioPath = chart.SoundFile,
                AudioPreviewTime = chart.PreviewStart,
                BackgroundPath = "",
                Description = "",
                MapId = chart.ID,
                MapSetId = 0,
                Creator = chart.Creator,
                DifficultyName = chart.Difficulty,
                Source = chart.Source,
                Tags = chart.Tags,
                SongLength = 0,
                NoteCount = 0,
                SliderCount = 0,
                SpinnerCount = 0
            };

            if (!skipPathSetting)
            {
                try
                {
                    map.Md5Checksum = MapsetHelper.GetMd5Checksum(path);
                    map.Directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? throw new InvalidOperationException()).Name.Replace("\\", "/");
                    map.Path = System.IO.Path.GetFileName(path)?.Replace("\\", "/");
                    map.LastFileWrite = File.GetLastWriteTimeUtc(map.Path);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {

                }
            }

            try
            {
                map.Bpm = chart.BPM;
            }
            catch (Exception)
            {
                map.Bpm = 0;
            }

            map.DateAdded = DateTime.Now;
            return map;
        }

        /// <summary>
        ///     Loads the .chart, .osu or .sm file for a map.
        ///
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ChartFile LoadChart(bool checkValidity = true)
        {
            // Reference to the parsed .chart file
            ChartFile chart;

            // Handle osu! maps as well
            switch (Game)
            {
                case MapGame.DrumSmasher:
                    var chartPath = $"{GlobalConfig.Load().SongDirectory}/{Directory}/{Path}";
                    chart = ChartFile.Parse(chartPath, checkValidity);
                    break;
                case MapGame.Osu:
                    chart = null; //TODO: Load osu chart file
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            return chart;
        }

        /// <summary>
        /// Calculates the difficulty of the entire map
        /// </summary>
        public void CalculateDifficulties()
        {
            var chart = LoadChart(false);

            //TODO: Difficulty Rating Calculation
        }

        /// <summary>
        /// Retrieve the map's difficulty rating from given mods
        /// </summary>
        /// <returns></returns>
        [Obsolete("Not yet Implemented")]
        public double DifficultyFromMods(ModIdentifier mods)
        {
            throw new NotImplementedException();
        }

    }
}
