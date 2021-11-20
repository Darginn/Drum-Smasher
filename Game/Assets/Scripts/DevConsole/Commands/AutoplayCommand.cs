using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;
using Assets.Scripts.TaikoGame;

namespace Assets.Scripts.DevConsole.Commands
{
    public class AutoplayCommand : BaseCommand
    {
        [AutoInit]
        public static void Init()
        {
            DevConsole.Commands.Add(new AutoplayCommand());
        }

        public override string Command => "ap";

        public override void Execute(params string[] args)
        {
            TaikoConfig taiko = (TaikoConfig)ConfigManager.GetOrLoadOrAdd<TaikoConfig>();

            if (args == null || args.Length == 0)
            {
                DevConsole.WriteLine("Autoplay: " + taiko.Autoplay);
                return;
            }

            if (!bool.TryParse(args[0], out bool newAP))
            {
                DevConsole.WriteLine($"Could not parse {args[0]} to float!");
                return;
            }

            bool oldAP = taiko.Autoplay;
            ActiveTaikoSettings.IsAutoplayActive = newAP;
            taiko.Save();

            DevConsole.WriteLine($"Changed Autoplay from {oldAP} to {newAP}");
        }
    }
}
