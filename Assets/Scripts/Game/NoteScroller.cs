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
using System.Collections;
using Assets.Scripts.Game.Mods;
using DSServerCommon;

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
        [SerializeField] List<(string, float)> _currentMods;

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
            if (!_reachedEndOfChart || _lastNote != null || _notesIndex <= 0)
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
            AsyncOperation loadAO = SceneManager.LoadSceneAsync("Main");
            loadAO.completed += ao =>
            {
                GameManager.OnSceneLoaded(_chart, _chartDirectory, _currentMods);
            };
        }

        public void Exit()
        {
            SceneManager.LoadScene("TitleScreen");
        }

        private void Reset()
        {
            _scoreScreen.Reset();
            _conductor.Stop();
            _layer = 0f;
            _statisticHandler.Reset();
            _reachedEndOfChart = false;
            _notesIndex = 0;
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
        public void LoadChart(Chart chart, DirectoryInfo chartDirectory, bool startPlaying = false, 
                              List<(string, float)> mods = null)
        {
            Reset();

            _notes = chart.Notes.ToList();
            _chart = chart;
            _speed = chart.Speed;

            _chartDirectory = chartDirectory;
            _conductor.LoadMp3File(Path.Combine(chartDirectory.FullName, chart.SoundFile));

            SetSpeed(_speed);
            LoadSettings();

            if (startPlaying)
                Play(mods);

        }

        public void SetAutoPlay(bool autoplay)
        {
            AutoPlay = autoplay;
            _key1Controller.AutoPlay = autoplay;
            _key2Controller.AutoPlay = autoplay;
            _key3Controller.AutoPlay = autoplay;
            _key4Controller.AutoPlay = autoplay;
        }

        public void LoadSettings()
        {
            Logger.Log("Loading Taiko Settings");

            Settings.TaikoSettings settings = (Settings.TaikoSettings)Settings.SettingsManager.SettingsStorage["Taiko"];

            if (settings.Data.Autoplay)
                SetAutoPlay(true);

            _key1Controller.KeyId = 1;
            _key2Controller.KeyId = 2;
            _key3Controller.KeyId = 3;
            _key4Controller.KeyId = 4;

            if (!Enum.TryParse(settings.Data.Key1.ToUpper(), out _key1Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key1}", LogLevel.Error);
            if (!Enum.TryParse(settings.Data.Key2.ToUpper(), out _key2Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key2}", LogLevel.Error);
            if (!Enum.TryParse(settings.Data.Key3.ToUpper(), out _key3Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key3}", LogLevel.Error);
            if (!Enum.TryParse(settings.Data.Key4.ToUpper(), out _key4Controller.KeyToPress))
                Logger.Log($"Could not parse key {settings.Data.Key4}", LogLevel.Error);

            Logger.Log("Loaded Taiko Settings");
        }

        /// <summary>
        /// Starts playing the chart
        /// </summary>
        public void Play(List<(string, float)> mods = null)
        {
            if (mods != null)
            {
                if (mods.Any(m => m.Item1.Equals("AutoPlayMod", StringComparison.CurrentCultureIgnoreCase)))
                    AutoPlay = true;

                GameObject modPanel = GameObject.Find("Mods");
                ModController[] modControllers = modPanel.GetComponentsInChildren<ModController>();
                Logger.Log(modControllers.Length.ToString());

                for (int i = 0; i < modControllers.Length; i++)
                    Logger.Log(modControllers[i].Name);

                for (int i = 0; i < mods.Count; i++)
                {
                    ModController controller = modControllers.First(mc => mc.Name.Equals(mods[i].Item1, StringComparison.CurrentCultureIgnoreCase));
                    
                    controller.ModObject.SetActive(true);
                    _statisticHandler.Multiplier += controller.BaseMod.Multiplier - 1f;

                    Logger.Log($"Enabled mod {mods[i]} {i + 1}/{mods.Count}");
                }

                _currentMods = mods;
            }

            if (AutoPlay)
            {
                _statisticHandler.Multiplier = 0f;
                _key1Controller.AutoPlay = true;
                _key2Controller.AutoPlay = true;
                _key3Controller.AutoPlay = true;
                _key4Controller.AutoPlay = true;
            }

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
