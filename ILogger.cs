using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DSServerCommon
{
    public interface ILogger
    {
        void Log(string message, LogLevel level = LogLevel.Info, [CallerMemberName] string callerName = "");
    }
}
