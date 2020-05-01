using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DrumSmasher.Charts;
using System.IO;
using DrumSmasher.GameInput;

namespace DrumSmasher.Game
{
    public class NoteScroller : MonoBehaviour 
    {
        private Chart _chart;
        private List<ChartNote> _notes;
        private int _notesIndex;
        private bool _reachedEndOfChart;

        [SerializeField]
        private SoundConductor _conductor;
        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _defaultSpeed = 6f;
        [SerializeField]
        private float _offset;
        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField]
        private Vector3 _hitCirclePosition;
        [SerializeField]
        private GameObject _notePrefab;
        [SerializeField]
        private ButtonController _key1Controller;
        [SerializeField]
        private ButtonController _key2Controller;
        [SerializeField]
        private ButtonController _key3Controller;
        [SerializeField]
        private ButtonController _key4Controller;

        private float _layer;

        void Start()
        {

        }

        void Update()
        {
            if (_conductor.PlayState != PlayState.Playing || _reachedEndOfChart)
                return;

            TrySpawnNote();
        }

        /// <summary>
        /// Sets the speed and current spawn offset
        /// </summary>
        /// <param name="speed">note speed, -1 uses <see cref="_defaultSpeed"/></param>
        public void SetSpeed(float speed = -1)
        {
            if (speed == -1)
                _speed = _defaultSpeed;
            else
                _speed = speed;

            float dist = _startPosition.x - _hitCirclePosition.x;

            _offset = dist / _speed;
        }

        /// <summary>
        /// Loads a chart
        /// </summary>
        /// <param name="chart">chart to load</param>
        public void LoadChart(Chart chart, DirectoryInfo chartDirectory)
        {
            _notes = chart.Notes.ToList();
            _notesIndex = 0;
            _chart = chart;
            _layer = 0f;
            _speed = chart.Speed;

            _conductor.LoadMp3File(Path.Combine(chartDirectory.FullName, chart.SoundFile));

            SetSpeed(_speed);
        }

        /// <summary>
        /// Starts playing the chart
        /// </summary>
        public void Play()
        {
            _conductor.Play();
            _reachedEndOfChart = false;
        }

        private (double, ChartNote) GetNextAvailableNote()
        {
            if (_notes.Count <= _notesIndex)
            {
                _reachedEndOfChart = true;
                return (-1, null);
            }

            double spawnTime = _notes[_notesIndex].Time.TotalSeconds - _offset;

            if (spawnTime > _conductor.CurrentTime)
                return (-1, null);

            int index = _notesIndex;
            _notesIndex++;

            return (spawnTime, _notes[index]);
        }

        private void TrySpawnNote()
        {
            (double, ChartNote) noteToSpawn = GetNextAvailableNote();

            if (noteToSpawn.Item2 == null)
                return;

            SpawnNote(noteToSpawn.Item2, noteToSpawn.Item1);
        }

        private void SpawnNote(ChartNote note, double startTime)
        {
            GameObject noteToSpawn = Instantiate(_notePrefab, gameObject.transform);
            Note noteScript = noteToSpawn.GetComponent<Note>();
            noteScript.SetStartZ(_layer);
            _layer += 0.01f;

            noteScript.SetDefaultPosition();

            NoteColor color = (NoteColor)note.Color;
            NoteType type = note.BigNote ? NoteType.Big : NoteType.Small;

            noteScript.SetNoteType(type, color);

            switch(color)
            {
                default:
                case NoteColor.Blue:
                    noteScript.Key1Controller = _key3Controller;
                    noteScript.Key2Controller = _key4Controller;
                    break;

                case NoteColor.Red:
                    noteScript.Key1Controller = _key1Controller;
                    noteScript.Key2Controller = _key2Controller;
                    break;
            }

            noteScript.Speed = _speed;
            noteScript.Conductor = _conductor;
            noteScript.StartTime = (float)startTime;

            //SpriteRenderer renderer = noteToSpawn.GetComponent<SpriteRenderer>();
            //renderer.sortingOrder = 2 + _notesIndex;
        }
    }
}
