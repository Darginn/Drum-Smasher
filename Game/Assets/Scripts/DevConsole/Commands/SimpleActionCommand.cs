using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DevConsole.Commands
{
    public class SimpleActionCommand : BaseCommand
    {
        public event Action<string[]> OnExecute;

        public override string Command { get; }

        public SimpleActionCommand(string cmd, Action<string[]> toInvoke) : base()
        {
            Command = cmd;
            OnExecute += toInvoke;
        }

        public override void Execute(params string[] args)
        {
            OnExecute?.Invoke(args);
        }
    }
}
