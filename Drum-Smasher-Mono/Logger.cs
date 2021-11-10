using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace Drum_Smasher_Mono
{
    public enum LogLevel
    {
        Info,
        Trace,
        Warning,
        Error
    }

    public static class Logger
    {
        static ConcurrentQueue<string> _logQueue;
        static Thread _logThread;
        static FileStream _logStream;

        static Logger()
        {
            _logQueue = new ConcurrentQueue<string>();
            _logStream = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _logThread = new Thread(new ThreadStart(LogThread));

            _logThread.Start();
        }

        public static void Log(string message, LogLevel level = LogLevel.Trace, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            Task.Run(() =>
            {
                string toLog = $"{DateTime.UtcNow}\t[{level}] ({line}) {caller}:\t{message}\n";
                Log(toLog);
            });
        }

        static void Log(string message)
        {
            _logQueue.Enqueue(message);
        }

        static void LogThread()
        {
            for(; ; )
            {
                while(_logQueue.TryDequeue(out string logMsg))
                {
                    _logStream.Write(Encoding.UTF8.GetBytes(logMsg));
                    Console.WriteLine(logMsg);
                }

                _logStream.Flush();

                // we don't need to constantly log messages, let them stack up a bit and then process them
                Thread.Sleep(100);
            }
        }
    }
}
