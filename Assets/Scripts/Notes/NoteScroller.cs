using DrumSmasher.Charts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Notes
{
    public class NoteScroller : MonoBehaviour
    {
        public GameTime GameTime { get { return _gameTime; } }
        public Conductor Sound;
        public NoteTracker Tracker;

        public Chart CurrentChart;
        public Chart LastChart;

        public Note OrigNoteBlue;
        public Note OrigNoteRed;
        
        public Text ScoreText;
        public Text ComboText;

        public Text Key1Text;
        public Text Key2Text;
        public Text Key3Text;
        public Text Key4Text;

        public Text AccuracyText;

        public GameInput.ButtonController Key1;
        public GameInput.ButtonController Key2;
        public GameInput.ButtonController Key3;
        public GameInput.ButtonController Key4;

        public Text SoundOffsetText;
        public long SoundOffset;

        public int Key1Hits;
        public int Key2Hits;
        public int Key3Hits;
        public int Key4Hits;

        public bool Paused;
        public bool Play;
        public bool AutoPlay;
        public bool ReachedEnd;
        public float NoteSpeed;
        public int PauseKeyDelayMS;
        public long Offset;
        public double AutoPlayDelayMS;

        public float BPM;
        public float CurrentBeat;
        
        private List<ChartNote> _notesToSpawn;
        private List<Note> _spawnedNotes;
        private GameTime _gameTime;
        private Vector3 _startPos;
        private DateTime _songStart;
        private DateTime _nextPause;
        private DateTime _pausedAt;
        private float _noteLayer;
        private bool _firstOffsetCheck;

        public NoteScroller() : base()
        {

        }

        void Start()
        {
            _spawnedNotes = new List<Note>();
            _gameTime = new GameTime();
            _notesToSpawn = new List<ChartNote>();
            Tracker = new NoteTracker()
            {
                Scroller = this,
                ComboText = ComboText,
                ScoreText = ScoreText,
                AccuracyText = AccuracyText,
                Key1Text = Key1Text,
                Key2Text = Key2Text,
                Key3Text = Key3Text,
                Key4Text = Key4Text,
                Key1 = Key1.KeyToPress,
                Key2 = Key2.KeyToPress,
                Key3 = Key3.KeyToPress,
                Key4 = Key4.KeyToPress,
            };

            Key1Text.text = "";
            Key2Text.text = "";
            Key3Text.text = "";
            Key4Text.text = "";
        }
        

        void Update()
        {
            //Start music based on offset
            if (Sound.Audio.PlaybackState != uAudio.uAudio_backend.PlayBackState.Playing && _songStart.Ticks <= DateTime.Now.Ticks)
                PlayMusic();

            if (!Play || ReachedEnd)
                return;

            SoundOffsetText.text = "Offset: " + _gameTime.ElapsedMilliseconds;
            SoundOffset = (long)_gameTime.ElapsedMilliseconds;

            //120 bpm
            //2 beats / s
            // 120 / 60 = 2 beats per second
            //1 / 2 = 0.5 s â beat

            if (BPM > 0 && Sound.Audio.IsPlaying)
            {
                float beatsPerSec = BPM / 60f;
                float secPerBeat = 1 / beatsPerSec;

                double beatPos = (Sound.Audio.CurrentTime.TotalSeconds / secPerBeat) - (Offset / 1000f / secPerBeat);
                CurrentBeat = (float)beatPos;
            }
            
            if (_nextPause < DateTime.Now && Input.GetKeyDown(KeyCode.Space))
            {
                if (Paused)
                {
                    long difference;
                    foreach(Note n in _spawnedNotes)
                    {
                        if (n == null)
                            continue;

                        difference = _pausedAt.Subtract(n.StartTime).Ticks;
                        n.StartTime = new DateTime(DateTime.Now.Ticks - difference);
                    }

                    Paused = false;
                    UnPauseMusic();
                    _gameTime.Start();
                }
                else
                {
                    Paused = true;
                    _pausedAt = DateTime.Now;
                    PauseMusic();
                    _gameTime.Stop();
                }

                _nextPause = DateTime.Now.AddMilliseconds(PauseKeyDelayMS);
            }

            if (Paused)
                return;

            //Check for chart end
            if (_notesToSpawn.Count == 0 && _spawnedNotes.Count > 0)
            {
                int notesLeft = _spawnedNotes.Count(n => n != null);

                if (notesLeft == 0)
                {
                    ReachedEnd = true;
                    return;
                }
            }

            //Try to spawn next note
            TrySpawnNote();
        }

        private void PlayMusic()
        {
            Logger.Log("Playing song " + Sound.Audio.targetFile);

            if (Paused)
            {
                UnPauseMusic();
                return;
            }
            if (Sound.Audio.IsPlaying)
                Sound.Audio.Stop();
            
            Sound.Audio.Play();
            Sound.Audio.StartSong();
        }
        private void StopMusic()
        {
            Logger.Log("Stopping song");

            if (!Sound.Audio.IsPlaying)
                return;

            Sound.Audio.Stop();
            Paused = false;
        }

        public void ReSkip(int amountMS)
        {
            TimeSpan oldTime = _gameTime.Time;

            _gameTime.RemoveTime(TimeSpan.FromMilliseconds(amountMS));

            var notes = CurrentChart.Notes.Where(n => n.Time.TotalMilliseconds > _gameTime.ElapsedMilliseconds && n.Time <= oldTime)
                                          .OrderBy(n => n.Time);

            _notesToSpawn.InsertRange(0, notes);
        }

        public void Skip(int amountMS)
        {
            _gameTime.AddTime(TimeSpan.FromMilliseconds(amountMS));

            for (int i = 0; i < _notesToSpawn.Count; i++)
                if (_notesToSpawn[i].Time < _gameTime.Time.Add(TimeSpan.FromMilliseconds(Offset)))
                    _notesToSpawn.RemoveAt(i);
        }

        private void UnPauseMusic()
        {
            if (Sound.Audio.PlaybackState != uAudio.uAudio_backend.PlayBackState.Paused)
            {
                if (Sound.Audio.IsPlaying)
                    return;
                else
                {
                    PlayMusic();
                    return;
                }
            }

            Sound.Audio.Resume();
            Paused = false;
        }

        private void PauseMusic()
        {
            if (!Sound.Audio.IsPlaying || Sound.Audio.PlaybackState == uAudio.uAudio_backend.PlayBackState.Paused)
                return;

            Sound.Audio.Pause();
            Paused = true;
        }

        private void TrySpawnNote()
        {
            if (_notesToSpawn.Count == 0 || _notesToSpawn.ElementAt(0).Time.Ticks > _gameTime.ElapsedTicks)
                return;

            ChartNote cn = _notesToSpawn[0];
            _notesToSpawn.RemoveAt(0);

            float start = (NoteSpeed * 4f * 2f) + 0f;

            Note origNote;
            origNote = cn.Color == 0 ? OrigNoteBlue : OrigNoteRed;

            Note n = Instantiate<Note>(origNote);
            n.StartPosition = new Vector3(start, origNote.transform.position.y, origNote.transform.position.z - _noteLayer);
            n.StartTime = DateTime.Now;
            n.DefaultNote = false;
            _spawnedNotes.Add(n);
            _noteLayer -= 0.02f;

            if (cn.BigNote)
            {
                n.BigNote = true;
                n.transform.localScale = new Vector3(1.35f, 1.35f, 1);
            }
        }

        public void Load(Chart ch)
        {
            if (ch == null)
            {
                Logger.Log("Chart not found", LogLevel.ERROR);
                return;
            }

            if (CurrentChart != null)
                LastChart = CurrentChart;

            CurrentChart = ch;
            if (_spawnedNotes.Count > 0)
            {
                _spawnedNotes.ForEach(n => Destroy(n));
                _spawnedNotes.Clear();
            }

            if (_notesToSpawn.Count > 0)
                _notesToSpawn.Clear();

            List<ChartNote> notes = ch.Notes.OrderBy(n => n.Time).ToList();
            for (int i = 0; i < notes.Count; i++)
                _notesToSpawn.Add(notes[i]);

            Offset = ch.Offset;

            _noteLayer = 0f;
            BPM = ch.BPM;
        }

        public void StartPlaying()
        {
            Logger.Log("Starting to play", LogLevel.Trace);

            _songStart = DateTime.Now;

            Play = true;
            Paused = false;
            _gameTime.Start();
            _gameTime.RemoveTime(TimeSpan.FromMilliseconds(Offset));
        }
    }
}
