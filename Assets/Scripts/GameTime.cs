using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher
{
    public class GameTime
    {
        private Stopwatch _originalTime;
        private double _timeOffsetMS;

        public TimeSpan Time
        {
            get
            {
                return _originalTime.Elapsed.Add(TimeSpan.FromMilliseconds(_timeOffsetMS));
            }
        }

        public double ElapsedMilliseconds
        {
            get
            {
                return Time.TotalMilliseconds;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                return Time.TotalSeconds;
            }
        }

        public double ElapsedMinutes
        {
            get
            {
                return Time.TotalMinutes;
            }
        }

        public double ElapsedHours
        {
            get
            {
                return Time.TotalHours;
            }
        }

        public double ElapsedDays
        {
            get
            {
                return Time.TotalDays;
            }
        }

        public double ElapsedTicks
        {
            get
            {
                return Time.Ticks;
            }
        }

        public GameTime()
        {
            _originalTime = new Stopwatch();
        }

        public void Start()
        {
            _originalTime.Start();
        }

        public void Stop()
        {
            _originalTime.Stop();
        }

        public void RemoveTime(TimeSpan time)
        {
            _timeOffsetMS -= time.TotalMilliseconds;
        }

        public void AddTime(TimeSpan time)
        {
            _timeOffsetMS += time.TotalMilliseconds;
        }

        public void ResetTime()
        {
            _timeOffsetMS = 0;
            _originalTime.Stop();
            _originalTime.Reset();
            _originalTime.Start();
        }
    }
}
