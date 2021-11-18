using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;

namespace Assets.Scripts.DevConsole.Commands
{
    public class FPSCommand : BaseCommand
    {
        [AutoInit]
        public static void Init()
        {
            DevConsole.Commands.Add(new FPSCommand());
        }

        public override string Command => "fps";

        public override void Execute(params string[] args)
        {
            GlobalConfig tss = (GlobalConfig)ConfigManager.GetOrLoadOrAdd<GlobalConfig>();

            if (args == null || args.Length < 2)
            {
                DevConsole.WriteLine($"FPS Menu {tss.FPSMenu} Game {tss.FPSInGame}");
                DevConsole.WriteLine("fps menu/game/current IntValue");
                return;
            }

            int value = -1;

            if (!int.TryParse(args[1], out value))
            {
                DevConsole.WriteLine($"Could not parse {value} to int32");
                return;
            }

            if (args[0].Equals("menu", StringComparison.CurrentCultureIgnoreCase))
            {
                tss.FPSMenu = value;

                DevConsole.WriteLine("Updated menu fps");
            }
            else if (args[0].Equals("game", StringComparison.CurrentCultureIgnoreCase))
            {
                tss.FPSInGame = value;

                DevConsole.WriteLine("Updated game fps");
            }
            else if (args[0].Equals("current", StringComparison.CurrentCultureIgnoreCase))
            {
                if (tss.FPSMenu == Application.targetFrameRate)
                    tss.FPSMenu = value;
                else
                    tss.FPSInGame = value;

                Application.targetFrameRate = value;

                DevConsole.WriteLine("Updated current fps");
            }
            else
            {
                DevConsole.WriteLine($"Could not find '{args[0]}'");
                return;
            }
            tss.Save();
        }
    }
}

