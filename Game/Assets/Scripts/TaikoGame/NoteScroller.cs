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

        ChartFile _chart;

        Note _lastSpawnedNote;
        List<Note> _spawnedNotes;
        int _nextNoteIndex;

        bool _reachedChartEnd;

        public NoteScroller()
        {
            _spawnedNotes = new List<Note>();
        }

        public void OnSceneLoaded(DirectoryInfo chartFolder, ChartFile cf, List<(string, float)> mods)
        {
            _reachedChartEnd = false;
            ActiveTaikoSettings.Reset();
            ClearAllNotes();

            NotePool.ClearPool();
            
            Logger.Log("Loading taiko scene");

            ActiveTaikoSettings.Sound = _sound;
            ActiveTaikoSettings.Sound.Stop();
            _chart = cf;

            Logger.Log("Loading Taiko Settings");

            TaikoConfig settings = (TaikoConfig)ConfigManager.GetOrLoadOrAdd<TaikoConfig>();

            ActiveTaikoSettings.IsAutoplayActive = settings.Autoplay;
            ActiveTaikoSettings.NoteSpeed = cf.Speed;

            float dist = _startPosObj.position.x - _hitPosObj.position.x;
            ActiveTaikoSettings.NoteOffset = dist / cf.Speed;

            if (!Enum.TryParse(settings.Key1.ToUpper(), out KeyCode k1))
                Logger.Log($"Could not parse key {settings.Key1}", LogLevel.Error);
            else
                Hotkeys.RegisterKey(new Hotkey(HotkeyType.TaikoInnerLeft, k1))
                       .OnCheckedDown += OnKeyHit;

            if (!Enum.TryParse(settings.Key2.ToUpper(), out KeyCode k2))
                Logger.Log($"Could not parse key {settings.Key2}", LogLevel.Error);
            else
                Hotkeys.RegisterKey(new Hotkey(HotkeyType.TaikoInnerRight, k2))
                       .OnCheckedDown += OnKeyHit;

            if (!Enum.TryParse(settings.Key3.ToUpper(), out KeyCode k3))
                Logger.Log($"Could not parse key {settings.Key3}", LogLevel.Error);
            else
                Hotkeys.RegisterKey(new Hotkey(HotkeyType.TaikoOuterLeft, k3))
                       .OnCheckedDown += OnKeyHit;

            if (!Enum.TryParse(settings.Key4.ToUpper(), out KeyCode k4))
                Logger.Log($"Could not parse key {settings.Key4}", LogLevel.Error);
            else
                Hotkeys.RegisterKey(new Hotkey(HotkeyType.TaikoOuterRight, k4))
                       .OnCheckedDown += OnKeyHit;

            ActiveTaikoSettings.Sound.LoadMp3File(Path.Combine(chartFolder.FullName, cf.SoundFile));

            Logger.Log("Loaded Taiko Settings");
            Logger.Log("Taiko scene loaded, starting song...");

            ActiveTaikoSettings.Sound.Play();
        }

        void OnKeyHit(Hotkey key)
        {
            switch(key.Type)
            {
                default:
                case HotkeyType.TaikoOuterLeft:
                    _drumOuterLeft.Trigger();
                    break;

                case HotkeyType.TaikoInnerLeft:
                    _drumInnerLeft.Trigger();
                    break;

                case HotkeyType.TaikoInnerRight:
                    _drumInnerRight.Trigger();
                    break;

                case HotkeyType.TaikoOuterRight:
                    _drumOuterRight.Trigger();
                    break;
            }
        }

        void SpawnNote(ChartNote cn, double spawnTime)
        {
            _lastSpawnedNote = Note.CreateNew(_nextNoteIndex++, (float)spawnTime, (float)cn.HitTime.TotalSeconds,
                                    _startPosObj.position, _hitPosObj.position, _destroyPosObj.position,
                                    _hitsizeAreaObj.sizeDelta / 2, cn.IsBigNote, _defaultNoteSprite, (NoteColor)cn.Color);

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
            if (spawnTime > ActiveTaikoSettings.Sound.CurrentTime)
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
            if (ActiveTaikoSettings.Sound.PlayState != PlayState.Playing ||
                _reachedChartEnd)
                return;

            TrySpawnNote();
        }
    }
}
