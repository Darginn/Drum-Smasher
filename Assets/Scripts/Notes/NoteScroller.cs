using DrumSmasher.Charts;
using DrumSmasher.GameInput;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Notes
{
    public class NoteScroller : MonoBehaviour
    {
        public GameObject HitCircle;
        public GameObject EndLine;

        public GameObject TaikoNoteRed;
        public GameObject TaikoNoteRedBig;
        public GameObject TaikoNoteBlue;
        public GameObject TaikoNoteBlueBig;

        public AudioSource HitSound;

        public KeyCode Key1;
        public KeyCode Key2;
        public KeyCode Key3;
        public KeyCode Key4;

        public double HitWindowRange;
        public bool AutoPlay;
        public double AutoPlayDelayMS;

        /// <summary>
        /// based on <see cref="ApproachRate"/>
        /// </summary>
        public double NoteSpeed
        {
            get
            {
                return ApproachRate * 5;
            }
        }
        public double ApproachRate
        {
            get
            {
                return AR;
            }
            set
            {
                AR = value;

                _timeHitToEnd = Vector3.Distance(HitCircle.transform.position, EndLine.transform.position) / NoteSpeed;
                _timeStartToHit = Vector3.Distance(TaikoNoteBlue.transform.position, HitCircle.transform.position) / NoteSpeed;
            }
        }
        public static double AR;

        public ButtonController Key1Controller;
        public ButtonController Key2Controller;
        public ButtonController Key3Controller;
        public ButtonController Key4Controller;

        public GameTime GameTime;
        public Stopwatch RealTime;
        public Text RealTimeText;
        public Conductor GameSound;
        public NoteTracker Tracker;

        private double _timeStartToHit;
        private double _timeHitToEnd;

        private Chart _currentChart;
        private List<NoteInfo> _currentChartNotes;
        private int _currentNoteIndex;
        
        private float _noteLayer;

        public bool Active;
        
        public double MusicStart;

        private readonly Vector3 _bigNoteScale = new Vector3(1.35f, 1.35f, 1.35f);
        
        // Start is called before the first frame update
        void Start()
        {
            if (ApproachRate == 0)
            {
                AR = 6;
                ApproachRate = 6;
            }
        }

        /// <summary>
        /// Loads a chart
        /// </summary>
        /// <param name="chart">The chart to load</param>
        public void Load(Chart chart)
        {
            Reset();

            _currentChart = chart;

            _timeStartToHit = Vector3.Distance(TaikoNoteBlue.transform.position, HitCircle.transform.position) / NoteSpeed;
            _timeHitToEnd = Vector3.Distance(HitCircle.transform.position, EndLine.transform.position) / NoteSpeed;

            GameTime.RemoveTime(_timeStartToHit * 1000);
            MusicStart += _timeStartToHit * 1000;

            Note blue = TaikoNoteBlue.GetComponent<Note>();
            Note red = TaikoNoteRed.GetComponent<Note>();

            red.Key1 = Key1Controller;
            red.Key2 = Key2Controller;
            blue.Key1 = Key3Controller;
            blue.Key2 = Key4Controller;


            TaikoNoteRedBig = Instantiate(TaikoNoteRed);
            TaikoNoteBlueBig = Instantiate(TaikoNoteBlue);

            TaikoNoteRedBig.transform.localScale = _bigNoteScale;
            TaikoNoteBlueBig.transform.localScale = _bigNoteScale;

            TaikoNoteRedBig.transform.position = TaikoNoteBlue.transform.position;
            TaikoNoteBlueBig.transform.position = TaikoNoteBlue.transform.position;

            IOrderedEnumerable<ChartNote> sortedNotes = chart.Notes.OrderBy(n => n.Time.TotalMilliseconds);
            for (int i = 0; i < sortedNotes.Count(); i++)
            {
                ChartNote n = sortedNotes.ElementAt(i);
                _currentChartNotes.Add(new NoteInfo(n, n.Time.TotalMilliseconds + (_timeStartToHit * 1000)));
            }

            Tracker.Key1 = Key1;
            Tracker.Key2 = Key2;
            Tracker.Key3 = Key3;
            Tracker.Key4 = Key4;
            Tracker.GameTime = GameTime;
        }

        /// <summary>
        /// Starts playing
        /// </summary>
        /// 
        public void StartPlaying()
        {
            Active = true;

            GameTime.Start();

            StartCoroutine(StartMusicAsync());
            StartCoroutine(StartSpawningNotesAsync());
        }

        /// <summary>
        /// Resets the notescroller
        /// </summary>
        public void Reset()
        {
            GameSound.Stop();

            if (GameTime == null)
                GameTime = new GameTime();
            else
            {
                GameTime.Stop();
                GameTime.ResetTime();
            }

            if (RealTime == null)
                RealTime = new Stopwatch();
            else
            {
                RealTime.Stop();
                RealTime.Reset();
            }


            if (_currentChartNotes == null)
                _currentChartNotes = new List<NoteInfo>();
            else
                _currentChartNotes.Clear();

            _currentNoteIndex = 0;
            _noteLayer = 0f;
        }

        // Update is called once per frame
        void Update()
        {
        }

        private IEnumerator StartSpawningNotesAsync()
        {
            while(Active)
            {
                TrySpawnNextNote();

                yield return new WaitForSecondsRealtime(0.00005f);
            }
        }

        private IEnumerator StartMusicAsync()
        {
            while(Active)
            {
                if (MusicStart <= GameTime.ElapsedMilliseconds)
                {
                    Logger.Log("Starting music at " + GameTime.ElapsedMilliseconds);
                    GameSound.Play();
                    double left = GameTime.ElapsedMilliseconds - MusicStart;
                    MusicStart = double.MaxValue;

                    //Might help being more accurate

                    if (left > 0)
                    {
                        Logger.Log($"Reducing GameTime by {left} ms due to late audio start");
                        GameTime.RemoveTime(left);
                    }

                    yield break;
                }

                yield return new WaitForSeconds(0.0005f);
            }
        }

        /// <summary>
        /// Tries to spawn the next note
        /// </summary>
        private void TrySpawnNextNote()
        {
            if (_currentNoteIndex < 0 || _currentNoteIndex >= _currentChartNotes.Count ||
                _currentChartNotes[_currentNoteIndex].Time.TotalMilliseconds > GameTime.ElapsedMilliseconds /*GameSound.MusicSource.time * 1000*/)
                return;

            NoteInfo next = _currentChartNotes[_currentNoteIndex];
            _currentNoteIndex++;

            SpawnNote(next);
        }
  
        /// <summary>
        /// Spawns a note
        /// </summary>
        /// <param name="note">The note to spawn</param>
        private void SpawnNote(NoteInfo note)
        {
            GameObject nObj = Instantiate(note.Color == 0 ? (note.BigNote ? TaikoNoteBlueBig : TaikoNoteBlue) : (note.BigNote ? TaikoNoteRedBig : TaikoNoteRed));
            Note n = nObj.GetComponent<Note>();

            nObj.transform.position = new Vector3(TaikoNoteBlue.transform.position.x, TaikoNoteBlue.transform.position.y, TaikoNoteBlue.transform.position.z + _noteLayer);
            _noteLayer += 0.0002f;
            
            n.BigNote = note.BigNote;
            n.AutoPlay = AutoPlay;
            n.AutoPlayDelayMS = AutoPlayDelayMS;
            n.HitWindowRangeX = HitWindowRange;
            n.SpawnOffset = (float)note.Time.TotalMilliseconds;
            n.Audio = GameSound.MusicSource;
            n.StartPos = TaikoNoteBlue;

            n.Active = true;

            //Check if we are offbeat, if yes fix position
            double msDiff = (GameSound.MusicSource.time * 1000) - note.Time.TotalMilliseconds;
            
            n.TimeStartToHitS = (float)note.Time.TotalSeconds - GameSound.MusicSource.time;
            n.TimeToHitOrig = (float)note.Time.TotalSeconds;
            n.TimeHitToEndS = (float)_timeHitToEnd;

            StartCoroutine(n.StartMoving());
        }

        private class NoteInfo : ChartNote
        {
            public double SpawnOffset;

            public NoteInfo(ChartNote note, double spawnOffset) : base(note.Time, note.BigNote, note.Color)
            {
                SpawnOffset = spawnOffset;
            }
        }
    }
}
