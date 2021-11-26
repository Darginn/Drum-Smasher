using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DevConsole.Commands
{
    public class HelpCommand : BaseCommand
    {
        public override string Command => "help";

        public override void Execute(params string[] args)
        {
            foreach(BaseCommand cmd in DevConsole.Commands.Commands.Values)
            {
                DevConsole.WriteLine(cmd.Command);
            }
        }
    }
}
