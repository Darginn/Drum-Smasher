using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrumSmasher.Charts;
using System;
using System.Timers;
using DrumSmasher.Note;
using System.Diagnostics;
using System.Linq;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace DrumSmasher
{
    public class NoteScroll : MonoBehaviour
    {
        public bool GameStart;
        public bool Started;
        public float NoteTempo;
        public bool ReachedEnd;
        public Chart CurrentChart;
        public Chart LastChart;
        public event EventHandler OnChartEnd;
        public Stopwatch GameTime;

        public Text ComboText;
        public int Combo;
        public Text ScoreText;
        public long Score;

        public float MultiplierValue;

        public NoteObject BlueNote;
        public NoteObject RedNote;

        public Conductor Conductor;

        public bool AutoPlay;

        private long _startTime;
        
        private float _noteSpeed
        {
            get
            {
                return NoteTempo / 60f;
            }
        }
        private int _noteIndex;

        private List<NoteObject> _spawnedNotes;
        private Queue<ChartNote> _notesToSpawn;

        // Start is called before the first frame update
        void Start()
        {
            _spawnedNotes = new List<NoteObject>();
            GameTime = new Stopwatch();
            _notesToSpawn = new Queue<ChartNote>();
        }

        // Update is called once per frame
        void Update()
        {
            if (GameStart)
            {
                if (_startTime == 0)
                {
                    ChartNote cn = _notesToSpawn.ElementAt(0);
                    DateTime st = DateTime.Now.AddMilliseconds(CurrentChart.Offset);
                    _startTime = st.Ticks;
                    Logger.Log("Start time set to " + st.ToString());
                }

                StartGame();
                return;
            }
            else if ((!GameStart && !Started) || ReachedEnd)
                return;
            

            //Reached end of chart, stop playing
            if (_noteIndex > CurrentChart.Notes.Count - 1)
            {
                foreach (NoteObject no in _spawnedNotes)
                    if (no.CanBeHit)
                        return;

                OnReachedChartEnd();
                return;
            }

            if (_notesToSpawn.ElementAt(0).Time.Ticks > GameTime.ElapsedTicks)
                return;
            
            ChartNote n = _notesToSpawn.Dequeue();
            NoteObject origNote = n.Color == 0 ? BlueNote : RedNote;
            NoteObject note = Instantiate(origNote, new Vector3(41.58999f, 2.468484f, 0f), new Quaternion());
            note.StartTime = DateTime.Now;

            note.BigNote = n.BigNote;
            note.DefaultNote = false;
            note.NoteSpeed = _noteSpeed;
            note.OnNoteHit += new EventHandler<bool>(OnNoteHit);
            note.OnNoteMiss += new EventHandler(OnNoteMiss);
            note.AutoPlay = AutoPlay;

            //speed * time * 3 * end
            float start = (_noteSpeed * 4f * 2f) + 0f;
            note.StartPos = new Vector3(start, origNote.transform.position.y);
            _spawnedNotes.Add(note);
            _noteIndex++;

            if (_startTime <= DateTime.Now.Ticks)
            {
                Logger.Log("Starting music", LogLevel.Trace);
                _startTime = long.MaxValue;

                if (Conductor.MusicSource.isPlaying)
                    Conductor.MusicSource.Stop();

                Conductor.MusicSource.Play();
            }
        }

        private void StartGame()
        {
            Logger.Log("Starting game");
            Logger.Log("Notes to spawn: " + _notesToSpawn.Count, LogLevel.Trace);
            
            if (_notesToSpawn.Count == 0)
                return;

            _startTime = 0;
            GameStart = false;
            Started = true;

            MultiplierValue = 1;

            List<ChartNote> notes = CurrentChart.Notes.OrderBy(p => p.Time).ToList();
            for (int i = 0; i < notes.Count(); i++)
                _notesToSpawn.Enqueue(notes[i]);

            if (GameTime.IsRunning)
                GameTime.Stop();
            GameTime.Reset();
            GameTime.Start();

            Logger.Log("Game started");
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

            _startTime = 0;
        }

        private void OnNoteHit(object sender, bool goodHit)
        {
            Conductor.HitSource.Play();
            Combo++;
            ComboText.text = Combo.ToString() + "x";
            
            if (Combo < 10)
                Score += 300;
            else
                Score += (long)(Math.Min((float)Math.Round(Combo / 10.0, MidpointRounding.AwayFromZero), 10f) * Math.Round(MultiplierValue, MidpointRounding.AwayFromZero));

            ScoreText.text = Score.ToString();

            Logger.Log($"Note Hit! Combo: {Combo}, Score {Score}", LogLevel.Trace);
        }

        private void OnNoteMiss(object sender, EventArgs e)
        {
            Combo = 0;
            ComboText.text = "";

            Logger.Log($"Note Miss!", LogLevel.Trace);
        }

        private void OnReachedChartEnd()
        {
            Logger.Log("Reached chart end");

            Started = false;
            ReachedEnd = true;

            OnChartEnd?.Invoke(this, null);
        }
    }
}
