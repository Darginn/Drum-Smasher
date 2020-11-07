using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DrumSmasher.Charts;
using System.Linq;
using System;
using DSServerCommon;

namespace DrumSmasher
{
    public class SongScanning : MonoBehaviour
    {
        public List<SongInfo> Songs;
        private DirectoryInfo _songFolder;

        public void Awake()
        {
            _songFolder = new DirectoryInfo(Application.dataPath + "/../Charts/");
        }

        public void RefreshSongs()
        {
            if (!_songFolder.Exists)
            {
                Logger.Log("Directory not found! Creating new one", LogLevel.Warning);
                _songFolder.Create();
                Logger.Log($"Directory {_songFolder.FullName} created successfully");
            }

            Logger.Log($"Found {_songFolder.FullName}");
            Songs = ScanForSongsRecursive(_songFolder).ToList();
        }

        private IEnumerable<SongInfo> ScanForSongsRecursive(DirectoryInfo directory)
        {
            List<FileInfo> chartFiles = directory.EnumerateFiles("*.chart", SearchOption.AllDirectories).ToList();

            foreach (FileInfo chartFile in chartFiles)
            {
                Chart c = ChartFile.Load(chartFile.FullName);

                if (c == null)
                {
                    Logger.Log($"Could not load chart {chartFile.FullName}", LogLevel.Warning);
                    continue;
                }

                Logger.Log($"Loaded SongInfo: {c.Artist} - {c.Title} [{c.Difficulty}]");
                SongInfo si = new SongInfo($"{c.Artist}", $"{c.Title}", $"{c.Difficulty}", c, chartFile.Directory);

                yield return si;
            }
        }

        public class SongInfo
        {
            public string Artist;
            public string Name;
            public string Difficulty;
            public Chart chart;
            public DirectoryInfo ChartDirectory;

            public string DisplayName;

            public SongInfo(string displayArtist, string displayName, string difficulty, Chart c, System.IO.DirectoryInfo chartDirectory)
            {
                Artist = displayArtist;
                Name = displayName;
                Difficulty = difficulty;
                chart = c;
                ChartDirectory = chartDirectory;
                DisplayName = displayArtist + " - " + displayName + $" [{difficulty}]";
            }

            public SongInfo()
            {
            }
        }
    }
}

