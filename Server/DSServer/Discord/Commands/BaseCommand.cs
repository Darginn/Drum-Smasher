using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Discord.Commands
{
    public abstract class BaseCommand<T> : IDisposable
    {
        public string Command { get; private set; }
        public bool IsDisposed { get; private set; }

        public BaseCommand(string command)
        {
            Command = command;
        }

        public virtual void Initialize()
        {

        }

        public virtual void Dispose()
        {

            IsDisposed = true;
        }

        public abstract void OnCommand(T args);
    }
}
