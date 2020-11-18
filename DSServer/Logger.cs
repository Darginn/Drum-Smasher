using DSServerCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

public static class Logger
{
    static DSServerCommon.Logger _logger;

    public static ILogger GetLogger()
    {
        if (_logger == null ||
            _logger.IsDisposed)
        {
            _logger = new DSServerCommon.Logger(true);
        }

        return _logger;
    }

    public static void Log(object message, LogLevel level = LogLevel.Debug, [CallerMemberName()] string caller = "")
        => GetLogger().Log(message.ToString(), level, caller);

    public static void Dispose()
    {
        _logger?.Dispose();
        _logger = null;
    }
}