using DrumSmasher.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Prefab.DevConsole.Commands
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
            TitleScreenSettings tss = (TitleScreenSettings)SettingsManager.SettingsStorage["TitleScreen"];

            if (args == null || args.Length < 2)
            {
                DevConsole.WriteLine($"FPS Menu {tss.Data.FPSMenu} Game {tss.Data.FPSInGame}");
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
                tss.Data.FPSMenu = value;

                DevConsole.WriteLine("Updated menu fps");
            }
            else if (args[0].Equals("game", StringComparison.CurrentCultureIgnoreCase))
            {
                tss.Data.FPSInGame = value;

                DevConsole.WriteLine("Updated game fps");
            }
            else if (args[0].Equals("current", StringComparison.CurrentCultureIgnoreCase))
            {
                if (tss.Data.FPSMenu == Application.targetFrameRate)
                    tss.Data.FPSMenu = value;
                else
                    tss.Data.FPSInGame = value;

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

