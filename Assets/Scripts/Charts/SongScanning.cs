using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DrumSmasher.Charts;
using System.Linq;
using System;

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
                Logger.Log("Directory not found! Creating new one", LogLevel.WARNING);
                _songFolder.Create();
                Logger.Log($"Directory {_songFolder.FullName} created successfully");
            }

            Logger.Log($"Found {_songFolder.FullName}");
            Songs = ScanForSongsRecursive(_songFolder).ToList();

            //ScanForSongsRecursive(_songFolder);
        }

        /*private void ScanForSongsRecursive(object folder)
        {
            List<SongInfo> list = new List<SongInfo>();
            List<DirectoryInfo> foldersToScan = new List<DirectoryInfo>();
            foldersToScan.Add((DirectoryInfo)folder);
            while (foldersToScan.Count > 0)
            {
                DirectoryInfo[] currentScan = foldersToScan.ToArray();
                foldersToScan.Clear();
                for (int i = 0; i < currentScan.Length; ++i)
                {
                    foreach (FileInfo f in currentScan[i].GetFiles())
                    {
                        if (f.Name.EndsWith(".chart"))
                        {
                            Chart c = ChartFile.Load(f.FullName);
                            SongInfo si = new SongInfo($"{c.Artist} - {c.Title}", $"{c.Title} - {c.Artist}", c);
                            list.Add(si);
                            break;
                        }
                    }
                    foreach (DirectoryInfo d in currentScan[i].GetDirectories())
                    {
                        foldersToScan.Add(d);
                    }
                }
            }
            Songs = list;
        }*/

        private IEnumerable<SongInfo> ScanForSongsRecursive(DirectoryInfo directory)
        {
            List<FileInfo> chartFiles = directory.EnumerateFiles("*.chart", SearchOption.AllDirectories).ToList();

            foreach (FileInfo chartFile in chartFiles)
            {
                Chart c = ChartFile.Load(chartFile.FullName);

                if (c == null)
                {
                    Logger.Log($"Could not load chart {chartFile.FullName}", LogLevel.WARNING);
                    continue;
                }

                SongInfo si = new SongInfo($"{c.Artist} - {c.Title} [{c.Difficulty}]", $"{c.Title} [{c.Difficulty}] - {c.Artist}", c);

                yield return si;
            }
        }

        public class SongInfo
        {
            public string DisplayArtist;
            public string DisplayName;
            public string DisplayDifficulty;
            public Chart chart;

            public SongInfo(string displayArtist, string displayName, Chart c)
            {
                DisplayArtist = displayArtist;
                DisplayName = displayName;
                chart = c;
            }

            public SongInfo()
            {
            }
        }
    }
}

