using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace DrumSmasher
{
    public static class Logger
    {
        private static StreamWriter _logStream;
        private static object _logLock = new object();
        private static ConcurrentQueue<(string, LogLevel)> _queue;

        public static void Initialize(string logFile)
        {
            if (File.Exists(logFile))
            {
                if (File.Exists(logFile + 1))
                    File.Delete(logFile + 1);

                File.Move(logFile, logFile + 1);
            }
            
            _logStream = new StreamWriter(logFile);
            _queue = new ConcurrentQueue<(string, LogLevel)>();
        }


        public static void Log(string message, LogLevel level = LogLevel.Info, bool console = true, [CallerMemberName()] string caller = "")
        {
            if (_logStream == null)
                Initialize("log.txt");

            string time = $"{DateTime.UtcNow.Day}.{DateTime.UtcNow.Month}.{DateTime.UtcNow.Year} {DateTime.UtcNow.Hour}:{DateTime.UtcNow.Minute}:{DateTime.UtcNow.Second}:{DateTime.UtcNow.Millisecond}";
            string toLog = $"{time}: *{level}* {caller}: {message}";
            _queue.Enqueue((toLog, level));

            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(obj =>
            {
                lock(_logLock)
                {
                    if (!_queue.TryDequeue(out (string, LogLevel) p))
                        return;

                    _logStream.WriteLine(p.Item1);
                    _logStream.Flush();

#if DEBUG
                    if (console)
                        UnityEngine.Debug.Log(toLog);
#endif
                }
            }));

        }

        public static void Dispose()
        {
            if (_logStream != null)
                _logStream.Dispose();
        }

    }

    public enum LogLevel
    {
        Info,
        Trace,
        WARNING,
        ERROR
    }
}