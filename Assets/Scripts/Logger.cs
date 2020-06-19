using DSServerCommon;
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
        private static DSServerCommon.Logger _logger;

        public static ILogger GetLogger()
        {
            if (_logger == null ||
                _logger.IsDisposed)
            {
                _logger = new DSServerCommon.Logger(false);
#if UNITY_EDITOR
                _logger.AdditionLog += new Action<string, LogLevel>((s, l) =>
                {
                    switch(l)
                    {
                        case LogLevel.Error:
                            UnityEngine.Debug.LogError(s);
                            break;

                        case LogLevel.Warning:
                            UnityEngine.Debug.LogWarning(s);
                            break;

                        case LogLevel.Info:
                            UnityEngine.Debug.Log(s);
                            break;
                    }
                });
#endif
            }

            return _logger;
        }

        public static void Log(string message, LogLevel level = LogLevel.Info, [CallerMemberName()] string caller = "")
            => _logger.Log(message, level, caller);

        public static void Dispose()
        {
            _logger.Dispose();
            _logger = null;
        }
    }
}