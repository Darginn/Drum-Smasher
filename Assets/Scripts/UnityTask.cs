using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrumSmasher
{
    ///// <summary>
    ///// Thread-Safe class for running tasks in unity without having to check if we are on the editor
    ///// </summary>
    //public static class UnityTask
    //{
    //    private static List<(Task, CancellationTokenSource)> _tasks;
    //    private static object _taskLock = new object();

    //    public static void Run(Task task, CancellationTokenSource tokenSource)
    //    {
            
    //    }

    //    public static void DisposeAllTasks()
    //    {
    //        while (_tasks.Count > 0)
    //        {
    //            Task t = _tasks[0].Item1;

    //            if (t.IsCanceled || t.IsFaulted || t.IsCompleted)
    //            {
    //                _tasks.RemoveAt(0);
    //                continue;
    //            }

    //            _tasks[0].Item2.Cancel();
    //            _tasks.RemoveAt(0);
    //        }
    //    }
    //}
}
