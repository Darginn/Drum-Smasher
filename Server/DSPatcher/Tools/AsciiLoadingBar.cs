using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DSPatcher
{
    public class AsciiLoadingBar
    {
        public int RefreshTimeMs
        {
            get
            {
                lock (_loadingLock)
                {
                    return _refreshTimeMs;
                }
            }
            set
            {
                lock (_loadingLock)
                {
                    _refreshTimeMs = value;
                }
            }
        }

        public string Title
        {
            get
            {
                lock (_loadingLock)
                {
                    return _title;
                }
            }
            set
            {
                lock (_loadingLock)
                {
                    _title = value;
                }
            }
        }

        public string Description
        {
            get
            {
                lock (_loadingLock)
                {
                    return _description;
                }
            }
            set
            {
                lock (_loadingLock)
                {
                    _description = value;
                }
            }
        }

        public bool Active { get; private set; }

        public double MaxValue
        {
            get
            {
                lock (_loadingLock)
                {
                    return _maxValue;
                }
            }
            set
            {
                lock (_loadingLock)
                {
                    _maxValue = value;
                }
            }
        }

        public double CurrentValue
        {
            get
            {
                lock (_loadingLock)
                {
                    return _currentValue;
                }
            }
            set
            {
                lock (_loadingLock)
                {
                    _currentValue = value;
                }
            }
        }

        public double CurrentPercentage
        {
            get
            {
                lock (_loadingLock)
                {
                    return _currentPercentage;
                }
            }
            set
            {
                lock (_loadingLock)
                {
                    _currentPercentage = value;
                }
            }
        }

        private Task _refreshTask;
        private double _oldValue;
        private double _currentValue;
        private double _maxValue;
        private double _currentPercentage;

        private string _description;
        private string _title;
        private int _refreshTimeMs;

        private object _loadingLock = new object();

        public AsciiLoadingBar(int refreshTimeMs, double maxValue, double currentValue, string title)
        {
            RefreshTimeMs = refreshTimeMs;
            MaxValue = maxValue;
            CurrentValue = currentValue;
            Title = title;
        }

        private char[] _loadingSymbolStrings = new char[]
        {
            '|',
            '/',
            '—',
            '\\',
            '|',
            '/',
            '—',
            '\\'
        };

        private int _loadingSymbolsIndex;

        public void Increase(int amount)
        {
            CurrentValue += amount;
        }

        public void Decrease(int amount)
        {
            CurrentValue -= amount;
        }

        public void Increment()
        {
            CurrentValue++;
        }

        public void Decrement()
        {
            CurrentValue--;
        }

        public void Enable()
        {
            if (Active)
                return;

            Active = true;
            _oldValue = -1;

            _refreshTask = new Task(() => Refresh());
            _refreshTask.Start();
        }

        public void Disable()
        {
            if (!Active)
                return;

            Active = false;
        }

        private void Refresh()
        {
            while(Active)
            {
                lock (_loadingLock)
                {
                    if (_oldValue == CurrentValue)
                    {
                        Console.Clear();
                        Console.WriteLine(Title + '\n' + GetLine());

                        Task.Delay(RefreshTimeMs).Wait();
                        continue;
                    }

                    CurrentPercentage = GetPercentage(MaxValue, CurrentValue);

                    Console.Clear();
                    Console.WriteLine(Title + '\n' + GetLine());
                }

                Task.Delay(RefreshTimeMs).Wait();
            }
        }

        public void RefreshManually()
        {
            lock (_loadingLock)
            {
                if (_oldValue == CurrentValue)
                {
                    Console.Clear();
                    Console.WriteLine(Title + '\n' + GetLine());
                }

                CurrentPercentage = GetPercentage(MaxValue, CurrentValue);

                Console.Clear();
                Console.WriteLine(Title + '\n' + GetLine());
            }
        }

        private string GetLine()
        {
            StringBuilder sbuilder = new StringBuilder();

            int length = (int)Math.Truncate(CurrentPercentage / 5.0);

            sbuilder.Append('[');

            for (int i = 0; i < length; i++)
                sbuilder.Append('█');

            if (length < 20)
            {
                for (int i = length - 1; i < 20; i++)
                    sbuilder.Append(' ');
            }

            sbuilder.Append(']');

            char symbol = GetNextLoadingSymbol();

            sbuilder.Append($" {symbol} {CurrentValue}/{MaxValue} ({Math.Round(CurrentPercentage, 2, MidpointRounding.AwayFromZero)} %)");
            
            if (!string.IsNullOrEmpty(Description))
            {
                sbuilder.AppendLine();
                sbuilder.Append(Description);
            }

            return sbuilder.ToString();
        }

        private char GetNextLoadingSymbol()
        {
            char symbol = _loadingSymbolStrings[_loadingSymbolsIndex];
            _loadingSymbolsIndex++;

            if (_loadingSymbolsIndex >= _loadingSymbolStrings.Length)
                _loadingSymbolsIndex = 0;

            return symbol;
        }

        private double GetPercentage(double max, double value)
        {
            double result = 100 / max;
            return result * value;
        }
    }
}
