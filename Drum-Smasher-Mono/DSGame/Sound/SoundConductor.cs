using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NAudio.Wave;

namespace Drum_Smasher_Mono.DSGame.Sound
{
    public class SoundConductor
    {
        public TimeSpan CurrentPosition => _mp3reader?.CurrentTime ?? TimeSpan.Zero;
        public TimeSpan Length => _mp3reader.TotalTime;

        Mp3FileReader _mp3reader;
        WaveOut _mp3out;

        /// <summary>
        /// Plays the current song
        /// <para>To resume a song use <see cref="Resume"/></para>
        /// </summary>
        public void Play()
        {
            _mp3out?.Play();
        }

        /// <summary>
        /// Pauses the current song
        /// </summary>
        public void Pause()
        {
            _mp3out?.Pause();
        }

        /// <summary>
        /// Stops the current song
        /// </summary>
        public void Stop()
        {
            _mp3out?.Stop();
        }

        /// <summary>
        /// Resumes the current song
        /// </summary>
        public void Resume()
        {
            _mp3out?.Resume();
        }

        /// <summary>
        /// Loads an mp3 file so it can be played through <see cref="Play"/>
        /// </summary>
        /// <param name="mp3file"></param>
        public void Load(string mp3file)
        {
            if (!File.Exists(mp3file))
                throw new FileNotFoundException("Could not find mp3 file", mp3file);

            UnloadSong();

            _mp3reader = new Mp3FileReader(mp3file);
            _mp3out = new WaveOut();
            _mp3out.Init(_mp3reader);
        }

        /// <summary>
        /// Unloads the current song
        /// </summary>
        public void UnloadSong()
        {
            if (_mp3reader == null)
                return;

            _mp3out.Stop();
            _mp3out.Dispose();
            _mp3reader.Dispose();
            _mp3reader = null;
        }

        /// <summary>
        /// Jumps to a specific time in the current song
        /// </summary>
        /// <param name="time"></param>
        public void JumpToTime(TimeSpan time)
        {
            if (_mp3reader == null)
                return;

            _mp3reader.CurrentTime = time;
        }
    }
}
