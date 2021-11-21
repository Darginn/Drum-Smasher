using System;
using System.IO;
using NAudio.Wave;
using UnityEngine;

namespace Assets.Scripts.TaikoGame
{
    public class SoundConductor : MonoBehaviour
    {
        public static SoundConductor Instance { get; private set; }

        /// <summary>
        /// Current Song Time
        /// </summary>
        public double CurrentTime => _currentTime;
        /// <summary>
        /// Audio is loaded and ready to play
        /// </summary>
        public bool IsLoaded => _musicSource != null && _musicSource.clip != null;
        /// <summary>
        /// Current Playstate of the music
        /// </summary>
        public PlayState PlayState => _playState;
        public bool LoadedMp3 { get; set; }
        public float Length => _musicSource.clip.length;

        [SerializeField] AudioSource _musicSource;
        [SerializeField] double _dspSongTime;
        [SerializeField] double _currentTime;
        [SerializeField] PlayState _playState = PlayState.Stopped;
        [SerializeField] AudioSource _hitSoundNote;

        public SoundConductor()
        {
            Instance = this;
        }

        /// <summary>
        /// Starts playing the music
        /// </summary>
        public void Play()
        {
            if (_playState != PlayState.Stopped)
                return;

            _musicSource.Play();
            _dspSongTime = AudioSettings.dspTime;
            _playState = PlayState.Playing;
        }

        public void PlayHitSound()
        {
            if (_hitSoundNote != null && _hitSoundNote.clip != null)
                _hitSoundNote.Play();
        }

        /// <summary>
        /// Pauses the music
        /// </summary>
        public void Pause()
        {
            if (_playState != PlayState.Playing)
                return;

            _musicSource.Pause();
        }

        /// <summary>
        /// Resumes paused music
        /// </summary>
        public void Resume()
        {
            if (_playState != PlayState.Paused)
                return;

            _musicSource.UnPause();
        }

        /// <summary>
        /// Stops the music
        /// </summary>
        public void Stop()
        {
            if (_playState != PlayState.Playing)
                return;

            _musicSource.Stop();
        }

        /// <summary>
        /// Loads an mp3 file
        /// </summary>
        /// <param name="file">file path</param>
        public void LoadMp3File(string file)
        {
            LoadedMp3 = false;

            byte[] data = LoadAudioBytes(file);
            WAV wav = ConvertMP3DataToWAV(data);
            _musicSource.clip = ConvertWAVToAudioClip(wav);

            LoadedMp3 = true;
        }

        /// <summary>
        /// Raises the volume
        /// </summary>
        /// <param name="count">volume to raise</param>
        public void VolumeUp(int count = 5)
        {
            int newVolume = (int)(_musicSource.volume * 100f) + count;
            SetVolume(newVolume);
        }

        /// <summary>
        /// Lowers the volume
        /// </summary>
        /// <param name="count">volume to raise</param>
        public void VolumeDown(int count = 5)
        {
            int newVolume = (int)(_musicSource.volume * 100f) - count;
            SetVolume(newVolume);
        }

        /// <summary>
        /// Sets the volume
        /// </summary>
        /// <param name="value">volume to set, min 0, max 100</param>
        public void SetVolume(int value)
        {
            value = Math.Min(Math.Max(0, value), 100);

            _musicSource.volume = value / 100f;
        }


        byte[] LoadAudioBytes(string file)
        {
            using (FileStream fstream = File.OpenRead(file))
            {
                byte[] data = new byte[fstream.Length];

                fstream.Read(data, 0, data.Length);

                return data;
            }
        }

        WAV ConvertMP3DataToWAV(byte[] mp3Data)
        {
            using (MemoryStream mp3Stream = new MemoryStream(mp3Data))
            {
                using (Mp3FileReader mp3Audio = new Mp3FileReader(mp3Stream))
                {
                    using(WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(mp3Audio))
                    {
                        using (MemoryStream outputStream = new MemoryStream())
                        {
                            using (WaveFileWriter waveFileWriter = new WaveFileWriter(outputStream, waveStream.WaveFormat))
                            {
                                byte[] bytes = new byte[waveStream.Length];
                                waveStream.Position = 0;
                                waveStream.Read(bytes, 0, Convert.ToInt32(waveStream.Length));
                                waveFileWriter.Write(bytes, 0, bytes.Length);
                                waveFileWriter.Flush();
                            }

                            return new WAV(outputStream.ToArray());
                        }
                    }
                }
            }
        }

        AudioClip ConvertWAVToAudioClip(WAV wav)
        {
            AudioClip result;

            if (wav.ChannelCount == 2)
            {
                result = AudioClip.Create("AudioClip Name", wav.SampleCount, 2, wav.Frequency, false);
                result.SetData(wav.StereoChannel, 0);
            }
            else
            {
                result = AudioClip.Create("AudioClip Name", wav.SampleCount, 1, wav.Frequency, false);
                result.SetData(wav.LeftChannel, 0);
            }

            return result;
        }

        void Update()
        {
            if (_playState != PlayState.Playing)
                return;

            _currentTime = AudioSettings.dspTime - _dspSongTime;
        }

    }
}
