using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.TaikoGame.Notes;
using Assets.Scripts.IO.Charts;
using Assets.Scripts.Configs.GameConfigs;
using Assets.Scripts.Configs;
using Assets.Scripts.Controls;
using System.IO;

namespace Assets.Scripts.TaikoGame
{
    public class NoteScroller : MonoBehaviour
    {
        public static NoteScroller Instance { get; private set; }

        [SerializeField] Transform _startPosObj;
        [SerializeField] Transform _hitPosObj;
        [SerializeField] Transform _destroyPosObj;
        [SerializeField] RectTransform _hitsizeAreaObj;

        [SerializeField] SoundConductor _sound;

        [SerializeField] TaikoDrum _drumOuterLeft;
        [SerializeField] TaikoDrum _drumInnerLeft;
        [SerializeField] TaikoDrum _drumInnerRight;
        [SerializeField] TaikoDrum _drumOuterRight;

        [SerializeField] Sprite _defaultNoteSprite;
        [SerializeField] Sprite _defaultNoteOverlaySprite;

        ChartFile _chart;
        DirectoryInfo _chartDirectory;

        Note _lastSpawnedNote;
        List<Note> _spawnedNotes;
        int _nextNoteIndex;

        bool _reachedChartEnd;

        public NoteScroller()
        {
            Instance = this;
            _spawnedNotes = new List<Note>();
        }

        public void OnSceneLoaded(DirectoryInfo chartFolder, ChartFile cf, List<(string, float)> mods)
        {
            _reachedChartEnd = false;
            ActiveTaikoSettings.Reset();
            ClearAllNotes();
            StatisticHandler.Instance.Reset();
            ScoreScreen.Instance.Hide();

            NotePool.ClearPool();
            
            Logger.Log("Loading taiko scene");

            SoundConductor.Instance.Stop();
            _chart = cf;
            _chartDirectory = chartFolder;

            Logger.Log("Loading Taiko Settings");

            TaikoConfig settings = (TaikoConfig)ConfigManager.GetOrLoadOrAdd<TaikoConfig>();

            ActiveTaikoSettings.IsAutoplayActive = settings.Autoplay;
            ActiveTaikoSettings.NoteSpeed = cf.Speed;

            float dist = _startPosObj.position.x - _hitPosObj.position.x;
            ActiveTaikoSettings.NoteOffset = dist / cf.Speed;

            if (!Enum.TryParse(settings.Key1.ToUpper(), out KeyCode k1))
                Logger.Log($"Could not parse key {settings.Key1}", LogLevel.Error);
            else
                new Hotkey("TaikoInnerLeft", k1, HotkeyType.OnKeyDown)
                    .OnInvoked += OnKeyHit;

            if (!Enum.TryParse(settings.Key2.ToUpper(), out KeyCode k2))
                Logger.Log($"Could not parse key {settings.Key2}", LogLevel.Error);
            else
                new Hotkey("TaikoInnerRight", k2, HotkeyType.OnKeyDown)
                    .OnInvoked += OnKeyHit;

            if (!Enum.TryParse(settings.Key3.ToUpper(), out KeyCode k3))
                Logger.Log($"Could not parse key {settings.Key3}", LogLevel.Error);
            else
                new Hotkey("TaikoOuterLeft", k3, HotkeyType.OnKeyDown)
                    .OnInvoked += OnKeyHit;

            if (!Enum.TryParse(settings.Key4.ToUpper(), out KeyCode k4))
                Logger.Log($"Could not parse key {settings.Key4}", LogLevel.Error);
            else
                new Hotkey("TaikoOuterRight", k4, HotkeyType.OnKeyDown)
                    .OnInvoked += OnKeyHit;

            SoundConductor.Instance.LoadMp3File(Path.Combine(chartFolder.FullName, cf.SoundFile));

            Logger.Log("Loaded Taiko Settings");
            Logger.Log("Taiko scene loaded, starting song...");

            SoundConductor.Instance.Play();
        }

        public void ReloadScene()
        {
            SoundConductor.Instance.Stop();
            OnSceneLoaded(_chartDirectory, _chart, null);
        }

        public bool CanNoteBeHit(Note n)
        {
            return _spawnedNotes.Count > 0 && _spawnedNotes[0].OrderIndex == n.OrderIndex;
        }

        public void RemoveNoteFromSpawnedList(Note n)
        {
            for (int i = 0; i < _spawnedNotes.Count; i++)
            {
                if (_spawnedNotes[i].OrderIndex == n.OrderIndex)
                {
                    _spawnedNotes.RemoveAt(i);
                    return;
                }
            }
        }

        void OnKeyHit(Hotkey key)
        {
            switch(key.Id)
            {
                default:
                case "TaikoOuterLeft":
                    _drumOuterLeft.Trigger();
                    StatisticHandler.Instance.IncrementKey("TaikoOuterLeft");
                    break;

                case "TaikoInnerLeft":
                    _drumInnerLeft.Trigger();
                    StatisticHandler.Instance.IncrementKey("TaikoInnerLeft");
                    break;

                case "TaikoInnerRight":
                    _drumInnerRight.Trigger();
                    StatisticHandler.Instance.IncrementKey("TaikoInnerRight");
                    break;

                case "TaikoOuterRight":
                    _drumOuterRight.Trigger();
                    StatisticHandler.Instance.IncrementKey("TaikoOuterRight");
                    break;
            }
        }

        void SpawnNote(ChartNote cn, double spawnTime)
        {
            _lastSpawnedNote = Note.CreateNew(_nextNoteIndex++, (float)spawnTime, (float)cn.HitTime.TotalSeconds,
                                    _startPosObj.position, _hitPosObj.position, _destroyPosObj.position,
                                    _hitsizeAreaObj.sizeDelta / 2, cn.IsBigNote, _defaultNoteSprite, _defaultNoteOverlaySprite, (NoteColor)cn.Color);

            _spawnedNotes.Add(_lastSpawnedNote);
        }

        void TrySpawnNote()
        {
            // We have reached the end of our chart
            if (_nextNoteIndex >= _chart.Notes.Count)
            {
                _reachedChartEnd = true;
                return;
            }

            ChartNote cn = _chart.Notes[_nextNoteIndex];
            double spawnTime = cn.HitTime.TotalSeconds - ActiveTaikoSettings.NoteOffset;

            // We are too early, don't spawn our next note
            if (spawnTime > SoundConductor.Instance.CurrentTime)
                return;

            SpawnNote(cn, spawnTime);
        }

        void ClearAllNotes()
        {
            while(_spawnedNotes.Count > 0)
            {
                Note note = _spawnedNotes[0];
                note.Delete();

                _spawnedNotes.RemoveAt(0);
            }
        }

        void Update()
        {
            if (SoundConductor.Instance.PlayState != PlayState.Playing || _reachedChartEnd)
            {
                // Check if we reached the end of the chart
                if (_nextNoteIndex == _chart.Notes.Count && _spawnedNotes.Count == 0)
                {
                    // Prevent this from being run every update
                    _nextNoteIndex = 0;

                    ScoreScreen.Instance.SetScoreStatistic(StatisticHandler.Instance.GetScoreStatistics());
                    ScoreScreen.Instance.Show();
                }

                return;
            }

            StatisticHandler.Instance.UpdateSoundOffset((float)SoundConductor.Instance.CurrentTime);
            TrySpawnNote();
        }
    }
}
