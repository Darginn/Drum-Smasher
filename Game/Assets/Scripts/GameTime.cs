using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Assets.Scripts
{
    public class GameTime
    {
        Stopwatch _originalTime;
        double _timeOffsetMS;
        TimeSpan _elapsed => _originalTime.Elapsed;

        public bool Enabled
        {
            get
            {
                if (_originalTime == null)
                    return false;

                return _originalTime.IsRunning;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                double elapsedMS = ElapsedMilliseconds;

                if (elapsedMS < 0)
                    return TimeSpan.FromMilliseconds(elapsedMS * -1).Ticks * -1;

                return TimeSpan.FromMilliseconds(elapsedMS).Ticks;
            }
        }

        public double ElapsedMilliseconds
        {
            get
            {
                return _elapsed.TotalMilliseconds + _timeOffsetMS;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                return ElapsedMilliseconds / 1000.0;
            }
        }

        public double ElapsedMinutes
        {
            get
            {
                return ElapsedMilliseconds / 1000.0 / 60.0;
            }
        }

        public double ElapsedHours
        {
            get
            {
                return ElapsedMilliseconds / 1000.0 / 60.0 / 60.0;
            }
        }

        public double ElapsedDays
        {
            get
            {
                return ElapsedMilliseconds / 1000.0 / 60.0 / 60.0 / 24.0;
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
            RemoveTime(time.TotalMilliseconds);
        }

        public void RemoveTime(double ms)
        {
            _timeOffsetMS -= ms;
        }

        public void AddTime(TimeSpan time)
        {
            AddTime(time);
        }

        public void AddTime(double ms)
        {
            _timeOffsetMS += ms;
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
