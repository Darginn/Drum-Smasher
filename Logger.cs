using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace DSServerCommon
{
    public class Logger : ILogger, IDisposable
    {
        public bool IsDisposed { get; private set; }
        public Action<string, LogLevel> AdditionLog
        {
            get
            {
                lock(_lock)
                {
                    return _additionLog;
                }
            }
            set
            {
                lock(_lock)
                {
                    _additionLog = value;
                }
            }
        }


        private Action<string, LogLevel> _additionLog;
        private bool _logToConsole;
        private bool _logToFile;
        private string _logFile;

        /// <summary>
        /// Current Time, Message, Log Level, CallerName
        /// </summary>
        private ConcurrentQueue<(DateTime, string, LogLevel, string)> _queue;
        private readonly object _lock = new object();

        private StreamWriter _writer;

        public Logger(bool logToConsole, bool logToFile = true, string logFile = "log.txt")
        {
            lock(_lock)
            {
                _logToConsole = logToConsole;
                _logToFile = logToFile;
                _logFile = logFile;

                if (logToFile)
                {
                    if (!File.Exists(logFile))
                        _writer = File.CreateText(logFile);
                    else
                    {
                        if (File.Exists(logFile + ".1"))
                            File.Delete(logFile + ".1");

                        File.Move(logFile, logFile + ".1");
                        _writer = File.CreateText(logFile);
                    }
                }
            }
        }

        public void Log(string message, LogLevel level, [CallerMemberName] string callerName = "")
        {
            _queue.Enqueue((DateTime.UtcNow, message, level, callerName));
            ThreadPool.QueueUserWorkItem(new WaitCallback(o => LogNext()));
        }

        private void LogNext()
        {
            //Lock so we write in order
            lock(_lock)
            {
                if (!_queue.TryDequeue(out (DateTime, string, LogLevel, string) toLog))
                    return;

                string message = $"{toLog.Item1} {toLog.Item4} *{toLog.Item3}*: {toLog.Item2}";

                if (_logToConsole)
                {
                    bool colorChanged = false;
                    switch(toLog.Item3)
                    {
                        case LogLevel.Error:
                            colorChanged = true;
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;

                        case LogLevel.Warning:
                            colorChanged = true;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                    }

                    Console.WriteLine(message);

                    if (colorChanged)
                        Console.ForegroundColor = ConsoleColor.Cyan;
                }

                if (_logToFile)
                {
                    _writer?.WriteLine(message);
                    _writer?.Flush();
                }

                AdditionLog?.Invoke(message, toLog.Item3);
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            _writer.Dispose();
            IsDisposed = true;
        }
    }
}
