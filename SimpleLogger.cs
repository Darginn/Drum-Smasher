using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DSServerCommon
{
    public class SimpleLogger : ILogger
    {
        public void Log(string message, int logLevel = 0, [CallerMemberName] string callerName = "")
        {
            Task.Run(() => Console.WriteLine($"{DateTime.UtcNow} {message}"));
        }
    }
}
