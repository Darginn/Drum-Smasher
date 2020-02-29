using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher
{
    /// <summary>
    /// Can be used after <see cref="AutoInitAttribute"/> was initialized
    /// </summary>
    public class UnityTask : INotifyCompletion, IDisposable, IEquatable<UnityTask>
    {
        [AutoInit]
        public static void Init()
        {
            Settings.SettingsManager.OnExit += (s, e) => DisposeTasks();
        }

        private static List<UnityTask> _taskss;
        private static List<UnityTask> _tasks
        {
            get
            {
                if (_taskss == null)
                    _taskss = new List<UnityTask>();

                return _taskss;
            }
        }

        public static int ActiveCachedTasks
        {
            get
            {
                int counter = 0;

                if (_tasks.Count == 0)
                    return 0;

                for (int i = 0; i < _tasks.Count; i++)
                {
                    UnityTask t = _tasks[i];

                    if (t.IsRunning)
                        counter++;
                }

                return counter;
            }
        }

        public static int TotalCachedTasks
        {
            get
            {
                return _tasks?.Count ?? 0;
            }
        }

        private static object _taskLock = new object();
        
        public static UnityTask RunSync(Action ac)
        {
            UnityTask task = AddTask(ac);
            task.Start().Wait();

            return task;
        }
        
        public static UnityTask RunAsync(Action ac)
        {
            UnityTask task = AddTask(ac);
            task.Start();

            return task;
        }

        public static UnityTask Run(Action ac, bool sync = false)
        {
            if (sync)
                return RunSync(ac);

            return RunAsync(ac);
        }

        public static void DisposeTasks()
            => DisposeTasksAsync().Wait();

        public static UnityTask DisposeTasksAsync()
        {
            UnityTask task = new UnityTask(() =>
            {
                Logger.Log("Disposing all tasks", LogLevel.Trace);

                lock (_taskLock)
                {
                    if (_tasks == null)
                    {
                        Logger.Log("No tasks to dispose found");
                        return;
                    }

                    while (_tasks.Count > 0)
                    {
                        UnityTask t = _tasks[0];

                        if (t == null || t.Disposed || !t.IsRunning)
                        {
                            _tasks.RemoveAt(0);
                            continue;
                        }

                        Logger.Log("Disposing task: " + t.Id);

                        t.Dispose();
                        _tasks.RemoveAt(0);
                    }
                }
            });

            return task.Start();
        }

        public static void ReleaseUnusedTasksFromCache()
            => ReleaseUnusedTasksFromCacheAsync().Wait();

        public static UnityTask ReleaseUnusedTasksFromCacheAsync(bool withLock = true)
        {
            return new UnityTask(() =>
            {
                if (withLock)
                {
                    lock (_taskLock)
                    {
                        release();
                    }
                }
                else
                    release();
            });
            
            void release()
            {
                for (int i = 0; i < _tasks.Count; i++)
                {
                    UnityTask t = _tasks[i];

                    if (t.IsRunning || !t.IsCompleted)
                        continue;

                    _tasks.RemoveAt(i);
                }
            }
        }

        private static UnityTask AddTask(Action ac)
        {
            lock(_taskLock)
            {
                UnityTask task = new UnityTask(ac);
                
                _tasks.Add(task);

                return task;
            }
        }



        public bool IsRunning
        {
            get
            {
                if (_task == null)
                    return false;

                return !_task.IsCanceled && !_task.IsCompleted && !_task.IsFaulted;
            }
        }

        public bool IsCompleted
        {
            get
            {
                if (_task == null)
                    return false;

                return _task.IsCompleted;
            }
        }

        public bool Disposed { get; private set; }

        public int Id
        {
            get
            {
                if (_task == null)
                    return -1;

                return _task.Id;
            }
        }

        private Task _task;
        
        public UnityTask(Task t)
        {
            _task = t;
        }

        public UnityTask(Action ac)
        {
            _task = new Task(ac);
        }

        public UnityTask Start()
        {
            if (_task == null)
                return this;

            _task.Start();
            return this;
        }

        public void Wait()
        {
            if (_task == null)
                return;

            _task.Wait();
        }

        public void Cancel()
            => Dispose();

        public UnityTask GetAwaiter()
            => this;

        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        public void Dispose()
        {
            _task?.Dispose();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnityTask);
        }

        public bool Equals(UnityTask other)
        {
            return other != null &&
                   EqualityComparer<Task>.Default.Equals(_task, other._task) &&
                   EqualityComparer<Boolean>.Default.Equals(Disposed, other.Disposed);
        }

        public override int GetHashCode()
        {
            return 929373003 + EqualityComparer<Task>.Default.GetHashCode(_task)
                             + EqualityComparer<Boolean>.Default.GetHashCode(Disposed);
        }

        public static bool operator ==(UnityTask task1, UnityTask task2)
        {
            return EqualityComparer<UnityTask>.Default.Equals(task1, task2);
        }

        public static bool operator !=(UnityTask task1, UnityTask task2)
        {
            return !(task1 == task2);
        }
    }
}
