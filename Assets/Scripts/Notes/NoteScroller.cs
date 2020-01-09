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
        public Conductor Sound;
        public NoteTracker Tracker;

        public Chart CurrentChart;
        public Chart LastChart;

        public Note OrigNoteBlue;
        public Note OrigNoteRed;
        public Note OrigNoteBigBlue;
        public Note OrigNoteBigRed;

        public Text ScoreText;
        public Text ComboText;

        public bool Paused;
        public bool Play;
        public bool AutoPlay;
        public bool ReachedEnd;
        public float NoteSpeed;
        public int PauseKeyDelayMS;
        public long Offset;
        
        private Queue<ChartNote> _notesToSpawn;
        private List<Note> _spawnedNotes;
        private Stopwatch _playTime;
        private Vector3 _startPos;
        private DateTime _songStart;
        private DateTime _nextPause;
        private DateTime _pausedAt;
        private float _noteLayer;

        public NoteScroller() : base()
        {

        }

        void Start()
        {
            _spawnedNotes = new List<Note>();
            _playTime = new Stopwatch();
            _notesToSpawn = new Queue<ChartNote>();
            Tracker = new NoteTracker()
            {
                ComboText = ComboText,
                ScoreText = ScoreText,
            };
            
        }

        void Update()
        {
            if (!Play || ReachedEnd)
                return;

            if (_nextPause < DateTime.Now && Input.GetKeyDown(KeyCode.Space))
            {
                if (Paused)
                {
                    long diffrence;
                    foreach(Note n in _spawnedNotes)
                    {
                        if (n == null)
                            continue;

                        diffrence = _pausedAt.Subtract(n.StartTime).Ticks;
                        n.StartTime = new DateTime(DateTime.Now.Ticks - diffrence);
                    }

                    Paused = false;
                    UnPauseMusic();
                }
                else
                {
                    Paused = true;
                    _pausedAt = DateTime.Now;
                    PauseMusic();
                }

                _nextPause = DateTime.Now.AddMilliseconds(PauseKeyDelayMS);
            }

            if (Paused)
                return;

            //Start music based on offset
            if (!Sound.MusicSource.isPlaying && _songStart.Ticks <= DateTime.Now.Ticks)
                PlayMusic();

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
            if (Paused)
            {
                UnPauseMusic();
                return;
            }
            if (Sound.MusicSource.isPlaying)
                Sound.MusicSource.Stop();

            Sound.MusicSource.Play();
        }

        private void StopMusic()
        {
            if (!Sound.MusicSource.isPlaying)
                return;

            Sound.MusicSource.Stop();
        }

        private void UnPauseMusic()
        {
            Sound.MusicSource.UnPause();
        }

        private void PauseMusic()
        {
            Sound.MusicSource.Pause();
        }

        private void TrySpawnNote()
        {
            if (_notesToSpawn.Count == 0 || _notesToSpawn.ElementAt(0).Time.Ticks > _playTime.ElapsedTicks)
                return;

            ChartNote cn = _notesToSpawn.Dequeue();

            float start = (NoteSpeed * 4f * 2f) + 0f;

            Note origNote;
            if (cn.BigNote)
                origNote = cn.Color == 0 ? OrigNoteBigBlue : OrigNoteBigRed;
            else
                origNote = cn.Color == 0 ? OrigNoteBlue : OrigNoteRed;

            Note n = Instantiate<Note>(origNote);
            n.StartPosition = new Vector3(start, origNote.transform.position.y, origNote.transform.position.z - _noteLayer);
            n.StartTime = DateTime.Now;
            n.DefaultNote = false;
            _spawnedNotes.Add(n);
            _noteLayer -= 0.02f;
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
                _notesToSpawn.Enqueue(notes[i]);

            Offset = ch.Offset;

            _noteLayer = 0f;
        }

        public void StartPlaying()
        {
            Logger.Log("Starting to play", LogLevel.Trace);

            _songStart = DateTime.Now.AddMilliseconds(Offset);
            Play = true;
            Paused = false;
            _playTime.Start();
        }
    }
}
