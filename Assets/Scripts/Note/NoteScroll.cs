using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrumSmasher.Charts;
using System;
using System.Timers;
using DrumSmasher.Note;
using System.Diagnostics;
using System.Linq;

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

        public NoteObject BlueNote;
        public NoteObject RedNote;
        
        private float _noteSpeed
        {
            get
            {
                return NoteTempo / 60f;
            }
        }
        private int _noteIndex;

        private List<NoteObject> _spawnedNotes;
        private List<NoteObject> _endNotes;
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
                StartGame();
                return;
            }
            else if (!GameStart && !Started)
                return;

            for (int i = 0; i < _spawnedNotes.Count; i++)
            {
                if (_spawnedNotes[i].ReachedEnd)
                {
                    _endNotes.Add(_spawnedNotes[i]);
                    _spawnedNotes.RemoveAt(i);
                }
            }

            //Reached end of chart, stop playing
            if (_noteIndex > CurrentChart.Notes.Count - 1)
            {
                Logger.Log("Checking for chart end");
                foreach (NoteObject no in _spawnedNotes)
                    if (no.CanBeHit)
                        return;

                OnReachedChartEnd();
                return;
            }

            if (_notesToSpawn.ElementAt(0).Time.Ticks >= GameTime.ElapsedTicks)
                return;

            Logger.Log("Spawning note");
            
            try
            {

                ChartNote n = _notesToSpawn.Dequeue();
                NoteObject origNote = n.Color == 0 ? BlueNote : RedNote;
                NoteObject note = Instantiate(origNote, new Vector3(41.58999f, 2.468484f, 0f), new Quaternion());
                //ToDo add time fix incase we spawn late
                note.BigNote = n.BigNote;
                note.DefaultNote = false;
                note.NoteSpeed = _noteSpeed;
                _spawnedNotes.Add(note);
                _noteIndex++;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to spawn note: " + ex.ToString(), LogLevel.ERROR);
                return;
            }

            Logger.Log("Spawned note");
        }

        private void StartGame()
        {
            Logger.Log("Starting game");
            Logger.Log("Notes to spawn: " + _notesToSpawn.Count, LogLevel.Trace);

            if (_notesToSpawn.Count == 0)
                return;

            GameStart = false;
            Started = true;

            List<ChartNote> notes = CurrentChart.Notes.OrderBy(p => p.Time).ToList();
            for (int i = 0; i < notes.Count(); i++)
                _notesToSpawn.Enqueue(notes[i]);
            
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
