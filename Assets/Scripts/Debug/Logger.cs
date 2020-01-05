using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Log
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
            string toLog = $"{DateTime.UtcNow}: *{level}* {caller}: {message}";
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
                        Debug.Log(toLog);
#endif
                }
            }));

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