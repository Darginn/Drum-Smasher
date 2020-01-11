using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace DrumSmasher
{
    public class Conductor : MonoBehaviour
    {
        public float SongBpm;
        public float SecPerBeat;
        public float SongPosition;
        public float SongPositionInBeats;
        public float DspSongTime;
        public AudioSource MusicSource;
        public AudioSource HitSource;
        public float Offset;
        public bool ReachedEnd;
        public Text MusicPositionText;

        private DateTime _nextTimeJump;

        private AudioClip _musicClip
        {
            get
            {
                return MusicSource.clip;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            MusicSource = GetComponent<AudioSource>();

            SecPerBeat = 60f / SongBpm;

            DspSongTime = (float)AudioSettings.dspTime;
        }

        // Update is called once per frame
        void Update()
        {
            SongPosition = (float)(AudioSettings.dspTime - DspSongTime - Offset);

            SongPositionInBeats = SongPosition / SecPerBeat;
            
            if (MusicSource != null && MusicSource.time <= _musicClip.length)
            {
                TimeSpan pos = TimeSpan.FromSeconds(MusicSource.time);
                TimeSpan length = TimeSpan.FromSeconds(_musicClip.length);
                MusicPositionText.text = $"{pos.Hours}:{pos.Seconds}:{pos.Milliseconds}/{length.Hours}:{length.Seconds}:{length.Milliseconds}";
            }
        }

        
        public void LoadSong(string file)
        {
            Logger.Log("Loading song " + file);
            LoadSongFromFile(file);

            if (MusicSource.clip == null)
                Logger.Log("Failed to load audioclip");
            else
                Logger.Log("Loaded audio clip");
        }

        private IEnumerator LoadSongFromFile(string file)
        {
            using (UnityWebRequest wr = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.OGGVORBIS))
            {
                yield return wr.SendWebRequest();

                MusicSource.clip = DownloadHandlerAudioClip.GetContent(wr);
            }
        }
    }
}