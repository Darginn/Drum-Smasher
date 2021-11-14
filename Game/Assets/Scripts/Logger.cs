using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public enum LogLevel
{
    Info,
    Trace,
    Warning,
    Error
}

public static class Logger
{
    static ConcurrentQueue<LogArgs> _logQueue;
    static Thread _logThread;
    static FileStream _logStream;

    static Logger()
    {
        _logQueue = new ConcurrentQueue<LogArgs>();
        _logStream = new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        _logThread = new Thread(new ThreadStart(LogThread));

        _logThread.Start();
    }

    public static void Log(string message, LogLevel level = LogLevel.Trace, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
    {
        Task.Run(() =>
        {
            string toLog = $"{DateTime.UtcNow}\t[{level}] ({line}) {caller}:\t{message}\n";
            _logQueue.Enqueue(new LogArgs(level, toLog));
        });
    }

    static void LogThread()
    {
        for (; ; )
        {
            if (_logQueue.Count == 0) // reduce unnecessary cpu usage when there are no log messages
            {
                Thread.Sleep(100);
                continue;
            }

            while (_logQueue.TryDequeue(out LogArgs args))
            {
                byte[] toWrite = Encoding.UTF8.GetBytes(args.Message);
                _logStream.Write(toWrite, 0, toWrite.Length);

                switch(args.Level)
                {
                    case LogLevel.Error:
                        UnityEngine.Debug.LogError(args.Message);
                        break;

                    case LogLevel.Warning:
                        UnityEngine.Debug.LogWarning(args.Message);
                        break;

                    default:
                        UnityEngine.Debug.Log(args.Message);
                        break;
                }
            }

            _logStream.Flush();
        }
    }

    class LogArgs
    {
        public LogLevel Level { get; }
        public string Message { get; }

        public LogArgs(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
    }
}