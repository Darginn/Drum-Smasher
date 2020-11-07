using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Prefab.DevConsole.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public DevConsole DevConsole { get; set; }

        public abstract string Command { get; }

        public abstract void Execute(params string[] args);
    }
}
