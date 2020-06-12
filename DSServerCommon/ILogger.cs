using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DSServerCommon
{
    public interface ILogger
    {
        void Log(string message, int logLevel = 0, [CallerMemberName] string callerName = "");
    }
}
