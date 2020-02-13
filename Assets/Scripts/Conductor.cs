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
        public Text MusicPositionText;
        public uAudio.uAudioPlayer Audio;
        public Notes.NoteScroller Scroller;
        
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
            
            if (Audio != null)
            {
                if (Audio.CurrentTime <= Audio.TotalTime)
                {
                    TimeSpan pos = Audio.CurrentTime;
                    TimeSpan length = Audio.TotalTime;
                    MusicPositionText.text = $"{pos.Minutes}:{pos.Seconds}:{pos.Milliseconds}/{length.Minutes}:{length.Seconds}:{length.Milliseconds}";
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        VolumeDown(10);
                    else
                        VolumeDown();
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        VolumeUp(10);
                    else
                        VolumeUp();
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        ReSkip(10 * 1000);
                    else
                        ReSkip();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        Skip(10 * 1000);
                    else
                        Skip();
                }
            }
        }

        public void VolumeUp(int amount = 5)
        {
            Audio.ChangeCurrentVolume(Audio.Volume + (amount / 100));
        }

        public void VolumeDown(int amount = 5)
        {
            Audio.ChangeCurrentVolume(Audio.Volume - (amount / 100));
        }

        /// <summary>
        /// Goes forward in the track
        /// </summary>
        /// <param name="amountMS"></param>
        public void Skip(int amountMS = 5000)
        {
            if (amountMS <= 0)
                return;

            TimeSpan newTime = Audio.CurrentTime.Add(TimeSpan.FromMilliseconds(amountMS));

            if (newTime > Audio.TotalTime)
                newTime = Audio.TotalTime;

            Audio.ChangeCurrentTime(newTime);

            Scroller?.Skip(amountMS);
        }

        /// <summary>
        /// Goes back in the track
        /// </summary>
        /// <param name="amountMS"></param>
        public void ReSkip(int amountMS = 5000)
        {
            if (amountMS <= 0)
                return;

            TimeSpan newTime = TimeSpan.FromMilliseconds(Audio.CurrentTime.TotalMilliseconds - amountMS);

            if (newTime.TotalMilliseconds < 0)
                newTime = TimeSpan.FromMilliseconds(0);

            Audio.ChangeCurrentTime(newTime);
            Scroller?.ReSkip(amountMS);
        }
        
        public void LoadSong(string file)
        {
            Logger.Log("Loading song " + file);
            Audio.targetFile = file;
        }
    }
}