#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ST = System.Threading.Tasks.Task;

namespace DrumSmasher
{
    public class Task
    {
        private static List<ST> _sTasks;
        private static Thread _taskWatcher;
        private static bool _shouldWatchTasks;
        private static bool _isWatching;
        private static Stopwatch _watcherWatch;

        [AutoInit]
        public static void Init()
        {
            Logger.Log("Initializing UnityTasks");

            Settings.SettingsManager.OnExit += (s, e) => DisposeAll();
            
            _sTasks = new List<ST>();
            _shouldWatchTasks = true;
            _watcherWatch = new Stopwatch();

            new Task(() =>
            {
                while(true)
                {
                    Logger.Log("ABC");
                    Task.Delay(500);
                }
            }).RunAsync();

            Logger.Log("Initialized UnityTasks");
        }

        private static void WatchTasks()
        {
            _isWatching = true;
            _watcherWatch.Start();

            while(_shouldWatchTasks)
            {
                if (_watcherWatch.ElapsedMilliseconds <= 5)
                {
                    ST.Delay(1).Wait();
                    continue;
                }

                lock(((ICollection)_sTasks).SyncRoot)
                {
                    for (int i = 0; i < _sTasks.Count; i++)
                    {
                        if (_sTasks[i].Status != TaskStatus.RanToCompletion &&
                            _sTasks[i].Status != TaskStatus.Canceled &&
                            _sTasks[i].Status != TaskStatus.Faulted)
                            continue;

                        _sTasks.RemoveAt(i);
                        i--;
                    }
                }

                _watcherWatch.Reset();
                _watcherWatch.Restart();
            }

            _isWatching = false;
        }

        private static void DisposeAll()
        {
            Logger.Log("Disposing all tasks");
            try
            {
                _shouldWatchTasks = false;
                int timePassed = 0;
                _taskWatcher.Abort();
                
                while(_taskWatcher.ThreadState != System.Threading.ThreadState.Aborted ||
                      _taskWatcher.ThreadState != System.Threading.ThreadState.Stopped ||
                      _taskWatcher.ThreadState != System.Threading.ThreadState.Suspended ||
                      _taskWatcher.ThreadState != System.Threading.ThreadState.Unstarted)
                {
                    ST.Delay(5).Wait();
                    timePassed += 5;


                    if (timePassed >= 10 * 1000)
                        throw new Exception("Timeout while trying to dispose the watcher thread");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to dispose task watcher\n{ex}");
            }

            int failCount = 0;

            lock(((ICollection)_sTasks).SyncRoot)
            {
                for (int i = 0; i < _sTasks.Count; i++)
                {
                    if (_sTasks[i].Status == TaskStatus.RanToCompletion ||
                        _sTasks[i].Status == TaskStatus.Canceled ||
                        _sTasks[i].Status == TaskStatus.Faulted)
                        continue;

                    try
                    {
                        _sTasks[i].Dispose();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Logger.Log("Disposed all tasks");
        }

        private ST _task;

        public Task(Action action)
        {
            _task = new ST(action);
            lock (((ICollection)_sTasks).SyncRoot)
            {
                _sTasks.Add(_task);
            }
        }

        public Task (ST stask)
        {
            _task = stask;

            lock (((ICollection)_sTasks).SyncRoot)
            {
                _sTasks.Add(stask);
            }
        }

        public void Run()
        {
            _task.RunSynchronously();
        }

        public void RunAsync()
        {
            _task.Start();
        }

        public void Wait()
        {
            _task.Wait();
        }

        public static void Delay(int milliseconds)
        {
            Task t = new Task(ST.Delay(milliseconds));
            t.Wait();
        }
    }
}
#endif