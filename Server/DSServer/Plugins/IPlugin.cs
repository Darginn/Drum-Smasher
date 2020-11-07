using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Plugins
{
    public interface IPlugin
    {
        string Name { get; }

        bool Load();
        void Unload();
    }
}
