using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DrumSmasher.Charts;
using System.IO;
using DrumSmasher.GameInput;
using UnityEngine.SceneManagement;

namespace DrumSmasher.Game
{
    public class NoteScroller : MonoBehaviour 
    {
        private Chart _chart;
        private List<ChartNote> _notes;
        private int _notesIndex;
        private bool _reachedEndOfChart;

        [SerializeField] private SoundConductor _conductor;
        [SerializeField] private float _speed;
        [SerializeField] private float _defaultSpeed = 6f;
        [SerializeField] private float _offset;
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _hitCirclePosition;
        [SerializeField] private GameObject _notePrefab;
        [SerializeField] private ButtonController _key1Controller;
        [SerializeField] private ButtonController _key2Controller;
        [SerializeField] private ButtonController _key3Controller;
        [SerializeField] private ButtonController _key4Controller;
        [SerializeField] internal bool AutoPlay;
        [SerializeField] StatisticHandler _statisticHandler;
        [SerializeField] ScoreScreen _scoreScreen;

        private float _layer;
        private DirectoryInfo _chartDirectory;

        void Start()
        {

        }

        void Update()
        {
            if (_conductor.PlayState != PlayState.Playing || _reachedEndOfChart)
                return;

            TrySpawnNote();
        }

        void FixedUpdate()
        {
            if (!_reachedEndOfChart || _lastNote != null)
                return;

            EnableScoreScreen();
        }

        public void EnableScoreScreen()
        {
            if (!_reachedEndOfChart)
                _reachedEndOfChart = true;

            var stats = _statisticHandler.GetScoreStatistics();
            _scoreScreen.SetScoreStatistic(stats);
            _scoreScreen.Show();
        }

        public void Retry()
        {
            LoadChart(_chart, _chartDirectory);
            Play();
        }

        public void Exit()
        {
            SceneManager.LoadScene("TitleScreen");
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
            _scoreScreen.Reset();
            _statisticHandler.Reset();
            _conductor.Stop();
            _reachedEndOfChart = false;
            _notes = chart.Notes.ToList();
            _notesIndex = 0;
            _chart = chart;
            _layer = 0f;
            _speed = chart.Speed;

            _chartDirectory = chartDirectory;
            _conductor.LoadMp3File(Path.Combine(chartDirectory.FullName, chart.SoundFile));

            SetSpeed(_speed);

            LoadSettings();
        }

        public void LoadSettings()
        {
            Logger.Log("Loading Taiko Settings");

            Settings.TaikoSettings settings = (Settings.TaikoSettings)Settings.SettingsManager.SettingsStorage["Taiko"];
            AutoPlay = settings.Data.Autoplay;
            _key1Controller.AutoPlay = AutoPlay;
            _key2Controller.AutoPlay = AutoPlay;
            _key3Controller.AutoPlay = AutoPlay;
            _key4Controller.AutoPlay = AutoPlay;

            _key1Controller.KeyId = 1;
            _key2Controller.KeyId = 2;
            _key3Controller.KeyId = 3;
            _key4Controller.KeyId = 4;

            if (!Enum.TryParse(settings.Data.Key1, out _key1Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key1}", LogLevel.ERROR);
            if (!Enum.TryParse(settings.Data.Key2, out _key2Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key2}", LogLevel.ERROR);
            if (!Enum.TryParse(settings.Data.Key3, out _key3Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key3}", LogLevel.ERROR);
            if (!Enum.TryParse(settings.Data.Key4, out _key4Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key4}", LogLevel.ERROR);

            Logger.Log("Loaded Taiko Settings");
        }

        /// <summary>
        /// Starts playing the chart
        /// </summary>
        public void Play()
        {
            _conductor.Play();
            _reachedEndOfChart = false;
        }

        private GameObject _lastNote;

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
            noteScript.AutoPlay = AutoPlay;
            noteScript.StatisticHandler = _statisticHandler;

            switch (color)
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

            if (_notesIndex >= _notes.Count)
                _lastNote = noteScript.gameObject;
        }
    }
}
