using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DrumSmasher
{
    public class SongScanning : MonoBehaviour
    {
        public static List<SongInfo> AllSongs;

        public delegate void OnFinished(List<SongInfo> songs);
        public Image LoadingImage;
        private List<SongInfo> songs = null;
        System.Object LockObject = new System.Object();

        [System.Serializable]
        public class SongInfo
        {
            public FileInfo fileInfo;
            public string artist, name, displayArtist, displayName;
        }

        void Start()
        {
            Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;
            StartCoroutine(ScanAndContinue(delegate (List<SongInfo> songs)
            {
                AllSongs = songs;
            }));
        }

        private IEnumerator ScanAndContinue(OnFinished onFinished)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Song List", LoadSceneMode.Single);
            asyncLoad.allowSceneActivation = false;
            Thread thread = new Thread(ScanForSongsRecursively);
            thread.IsBackground = true;
            thread.Start(new DirectoryInfo(Application.dataPath).Parent);
            while (true)
            {
                yield return null;
                lock (LockObject)
                {
                    if (songs != null) break;
                }
            }
            thread.Abort();
            AllSongs = songs;
            while (asyncLoad.isDone)
            {
                Logger.Log("Still loading Scene: " + asyncLoad.progress);
                if (asyncLoad.progress >= 0.9) break;
                yield return null;
            }

            while (LoadingImage.color.a > 0)
            {
                LoadingImage.color -= new Color(0, 0, 0, Time.deltaTime);
                yield return null;
            }

            asyncLoad.allowSceneActivation = true;
        }

        private void ScanForSongsRecursively(object folder)
        {
            List<SongInfo> list = new List<SongInfo>();
            List<DirectoryInfo> foldersToScan = new List<DirectoryInfo>();
            foldersToScan.Add((DirectoryInfo)folder);
            while (foldersToScan.Count > 0)
            {
                DirectoryInfo[] currentScan = foldersToScan.ToArray();
                foldersToScan.Clear();
                for (int i = 0; i < currentScan.Length; i++)
                {
                    foreach (FileInfo f in currentScan[i].GetFiles())
                    {
                        if (f.Name == "song.chart")
                        {
                            list.Add(CreateSongInfo(currentScan[i]));
                            break;
                        }
                    }
                    foreach (DirectoryInfo d in currentScan[i].GetDirectories())
                    {
                        foldersToScan.Add(d);
                    }
                }
            }
            List<SongInfo> sortedList = Sort(list);
            lock (LockObject)
            {
                songs = sortedList;
            }
        }

        private List<SongInfo> Sort(List<SongInfo> songs)
        {
            Dictionary<string, SongInfo> songByArtists = new Dictionary<string, SongInfo>();
            List<string> artists = new List<string>();
            for(int i = 0; i < songs.Count; i++)
            {
                if(!songByArtists.ContainsKey(songs[i].displayArtist))
                {
                    artists.Add(songs[i].displayArtist);
                    songByArtists.Add(songs[i].displayArtist, songs[i]);
                }
            }
            artists.Sort();
            List<SongInfo> sortedList = new List<SongInfo>();
            for(int i = 0; i < artists.Count; i++)
            {
                sortedList.Add(songByArtists[artists[i]]);
            }
            return sortedList;
        }

        private SongInfo CreateSongInfo(DirectoryInfo folder)
        {
            SongInfo songInfo = new SongInfo();
            FileInfo ini = null;
            foreach(FileInfo f in folder.GetFiles())
            {
                if(f.Name == "song.chart")
                {
                    songInfo.fileInfo = f;
                    ini = f;
                    break;
                }
            }
            string[] lines = File.ReadAllLines(ini.FullName);
            for(int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Title")) songInfo.name = lines[i].Split("="[0])[0];
                if (lines[i].StartsWith("Artist")) songInfo.artist = lines[i].Split("="[0])[0];
                if (lines[i] == @"[/\]") break;
            }
            songInfo.displayArtist = songInfo.artist + " - " + songInfo.name;
            songInfo.displayName = songInfo.name + " - " + songInfo.artist;
            return songInfo;
        }
    }
}
