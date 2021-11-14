using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using NAudio;
using NAudio.Wave;

namespace Assets.Scripts.Sound
{
    public class SoundConductor : MonoBehaviour
    {
        public float SongBpm;
        public float SecPerBeat;
        public float SongPosition;
        public float SongPositionInBeats;
        public float DspSongTime;
        public AudioSource HitSource;
        public float Offset;
        public Text MusicPositionText;
        public AudioSource MusicSource;

        DateTime _nextTimeJump;

        AudioClip _musicClip
        {
            get
            {
                return MusicSource.clip;
            }
        }

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
        }

        void FixedUpdate()
        {
            if (MusicSource != null && _musicClip != null && MusicSource.time <= _musicClip.length)
            {
                TimeSpan pos = TimeSpan.FromSeconds(MusicSource.time);
                TimeSpan length = TimeSpan.FromSeconds(_musicClip.length);
                MusicPositionText.text = $"{pos.Minutes}:{pos.Seconds}:{pos.Milliseconds}/{length.Minutes}:{length.Seconds}:{length.Milliseconds}";
            }
        }

        public void Play()
        {
            MusicSource.Play();
        }

        public void Stop()
        {
            MusicSource.Stop();
        }

        public void VolumeUp(int amount = 5)
        {
            MusicSource.volume += (amount / 100);
            HitSource.volume += amount / 100;
        }

        public void VolumeDown(int amount = 5)
        {
            MusicSource.volume -= (amount / 100);
            HitSource.volume -= amount / 100;
        }
        
        public void LoadSong(string file)
        {
            Logger.Log("Loading song " + file);
            StartCoroutine(LoadSongFromFile(file));
        }

        IEnumerator LoadSongFromFile(string file)
        {
            WWW www = new WWW(file);

            yield return www;

            MusicSource.clip = NAudioPlayer.FromMp3Data(www.bytes);
        }
    }
}