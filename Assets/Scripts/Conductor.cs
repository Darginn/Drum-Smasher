using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace DrumSmasher
{
    public class Conductor : MonoBehaviour
    {
        public float SongBpm;
        public float SecPerBeat;
        public float SongPosition;
        public float SongPositionInBeats;
        public float DspSongTime;
        public AudioSource HitSource;
        public float Offset;
        public bool ReachedEnd;
        public Text MusicPositionText;
        public uAudio.uAudioPlayer Audio;

        private DateTime _nextTimeJump;
        
        // Start is called before the first frame update
        void Start()
        {
            SecPerBeat = 60f / SongBpm;

            DspSongTime = (float)AudioSettings.dspTime;
        }

        void Update()
        {
            SongPosition = (float)(AudioSettings.dspTime - DspSongTime - Offset);

            SongPositionInBeats = SongPosition / SecPerBeat;
            
            if (Audio != null && Audio.CurrentTime <= Audio.TotalTime)
            {
                TimeSpan pos = Audio.CurrentTime;
                TimeSpan length = Audio.TotalTime;
                MusicPositionText.text = $"{pos.Minutes}:{pos.Seconds}:{pos.Milliseconds}/{length.Minutes}:{length.Seconds}:{length.Milliseconds}";
            }
        }

        
        public void LoadSong(string file)
        {
            Logger.Log("Loading song " + file);
            Audio.targetFile = file;
        }
    }
}