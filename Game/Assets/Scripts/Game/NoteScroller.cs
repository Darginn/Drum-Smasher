using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts.Game.Notes;
using Assets.Scripts.Game.Mods;
using Assets.Scripts.GameInput;
using Assets.Scripts.IO.Charts;

namespace Assets.Scripts.Game
{
    public class NoteScroller : MonoBehaviour 
    {
        public static NoteScroller Instance;

        public bool IgnoreColor;
        public ChartFile CurrentChart => _chart;
        public GameObject NotePrefab => _notePrefab;

        ChartFile _chart;
        List<ChartNote> _notes;
        int _notesIndex;
        bool _reachedEndOfChart;

        [SerializeField] SoundConductor _conductor;
        [SerializeField] float _speed;
        [SerializeField] float _defaultSpeed = 6f;
        [SerializeField] float _offset;
        [SerializeField] Vector3 _startPosition;
        [SerializeField] Vector3 _hitCirclePosition;
        [SerializeField] GameObject _notePrefab;
        //[SerializeField] ButtonController _key1Controller;
        //[SerializeField] ButtonController _key2Controller;
        //[SerializeField] ButtonController _key3Controller;
        //[SerializeField] ButtonController _key4Controller;
        [SerializeField] internal bool AutoPlay;
        [SerializeField] StatisticHandler _statisticHandler;
        [SerializeField] ScoreScreen _scoreScreen;
        [SerializeField] List<(string, float)> _currentMods;

        [SerializeField] TaikoDrumHotKey _hotkeyDrumOuterLeft;
        [SerializeField] TaikoDrumHotKey _hotkeyDrumInnerLeft;
        [SerializeField] TaikoDrumHotKey _hotkeyDrumInnerRight;
        [SerializeField] TaikoDrumHotKey _hotkeyDrumOuterRight;

        float _endTime;
        float _layer;
        DirectoryInfo _chartDirectory;
        GameObject _lastNote;

        void Start()
        {
            Instance = this;
        }

        void Update()
        {
            if (_conductor.PlayState != PlayState.Playing || _reachedEndOfChart)
                return;

            if (!TrySpawnNote() && _endTime == 0 && _lastNote != null &&
                _lastNote.GetComponent<Note>().IsSlider)
            {
                Note n = _lastNote.GetComponent<Note>();
                _endTime = n.SliderDuration + n.StartTime + 2.5f;
            }
        }

        void FixedUpdate()
        {
            if (!_reachedEndOfChart || _lastNote != null || _notesIndex <= 0 ||
                (_endTime != 0 && _conductor.CurrentTime < _endTime))
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

        void Reset()
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
        public void LoadChart(ChartFile chart, DirectoryInfo chartDirectory, bool startPlaying = false, 
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
            _statisticHandler.Multiplier = 0f;
            AutoPlay = autoplay;
            _hotkeyDrumOuterLeft.IsAutoplayEnabled = autoplay;
            _hotkeyDrumInnerLeft.IsAutoplayEnabled = autoplay;
            _hotkeyDrumInnerRight.IsAutoplayEnabled = autoplay;
            _hotkeyDrumOuterRight.IsAutoplayEnabled = autoplay;
        }

        public void LoadSettings()
        {
            Logger.Log("Loading Taiko Settings");

            Settings.TaikoSettings settings = (Settings.TaikoSettings)Settings.SettingsManager.SettingsStorage["Taiko"];

            if (settings.Data.Autoplay)
                SetAutoPlay(true);

            if (!Enum.TryParse(settings.Data.Key1.ToUpper(), out KeyCode k1))
                Logger.Log($"Could not parse key {settings.Data.Key1}", LogLevel.Error);
            else
                _hotkeyDrumInnerLeft.Key = k1;

            if (!Enum.TryParse(settings.Data.Key2.ToUpper(), out KeyCode k2))
                Logger.Log($"Could not parse key {settings.Data.Key2}", LogLevel.Error);
            else
                _hotkeyDrumInnerRight.Key = k2;

            if (!Enum.TryParse(settings.Data.Key3.ToUpper(), out KeyCode k3))
                Logger.Log($"Could not parse key {settings.Data.Key3}", LogLevel.Error);
            else
                _hotkeyDrumOuterLeft.Key = k3;

            if (!Enum.TryParse(settings.Data.Key4.ToUpper(), out KeyCode k4))
                Logger.Log($"Could not parse key {settings.Data.Key4}", LogLevel.Error);
            else
                _hotkeyDrumOuterRight.Key = k4;

            Logger.Log("Loaded Taiko Settings");
        }

        /// <summary>
        /// Starts playing the chart
        /// </summary>
        public void Play(List<(string, float)> mods = null)
        {
            bool autoplay = false;

            if (mods != null)
            {
                if (mods.Any(m => m.Item1.Equals("AutoPlayMod", StringComparison.CurrentCultureIgnoreCase)))
                    autoplay = true;

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

                    BaseMod bm = controller.ModObject.GetComponentInChildren<BaseMod>();
                    bm.OnEnabled(this);

                    Logger.Log($"Enabled mod {mods[i]} {i + 1}/{mods.Count}");
                }

                _currentMods = mods;
            }

            if (autoplay)
                SetAutoPlay(true);

            _conductor.Play();
            _reachedEndOfChart = false;
        }

        bool TryGetNextAvailableNote(out NextNote note)
        {
            if (_notes.Count <= _notesIndex)
            {
                _reachedEndOfChart = true;
                note = default;
                return false;
            }

            double spawnTime = _notes[_notesIndex].HitTime.TotalSeconds - _offset;

            if (spawnTime > _conductor.CurrentTime)
            {
                note = default;
                return false;
            }

            int index = _notesIndex;
            _notesIndex++;

            note = new NextNote(spawnTime, _notes[index]);
            return true;
        }

        bool TrySpawnNote()
        {
            if (!TryGetNextAvailableNote(out NextNote note))
                return false;

            SpawnNote(ref note);
            return true;
        }

        void SpawnNote(ref NextNote note)
        {
            GameObject noteToSpawn = Instantiate(_notePrefab, gameObject.transform);
            Note noteScript = noteToSpawn.GetComponent<Note>();
            noteScript.SetStartZ(_layer);
            _layer += 0.01f;

            noteScript.SetDefaultPosition();

            NoteColor color = (NoteColor)note.Note.Color;
            NoteType type = note.Note.IsBigNote ? NoteType.Big : NoteType.Small;

            noteScript.SetNoteType(type, color);
            noteScript.AutoPlay = AutoPlay;
            noteScript.StatisticHandler = _statisticHandler;
            noteScript.NoteScroller = this;

            noteScript.Key1 = _hotkeyDrumOuterLeft;
            noteScript.Key2 = _hotkeyDrumInnerLeft;
            noteScript.Key3 = _hotkeyDrumInnerRight;
            noteScript.Key4 = _hotkeyDrumOuterRight;

            noteScript.IgnoreColor = IgnoreColor;

            noteScript.Speed = _speed;
            noteScript.Conductor = _conductor;
            noteScript.StartTime = (float)note.SpawnTime;
            
            if (_notesIndex >= _notes.Count)
                _lastNote = noteScript.gameObject;

            if (note.Note.IsSlider)
                noteScript.ConvertToSlider((float)note.Note.SliderDuration.TotalSeconds, CurrentChart.BPM);
        }

        struct NextNote
        {
            public double SpawnTime { get; }
            public ChartNote Note { get; }

            public NextNote(double spawnTime, ChartNote note) : this()
            {
                SpawnTime = spawnTime;
                Note = note;
            }
        }
    }
}
