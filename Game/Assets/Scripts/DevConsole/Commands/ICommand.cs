﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Assets.Scripts.DevConsole.Commands
{
    public interface ICommand
    {
        DevConsole DevConsole { get; set; }
        string Command { get; }
        void Execute(params string[] args);
    }
}
