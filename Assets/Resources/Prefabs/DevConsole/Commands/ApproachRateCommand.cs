using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Prefab.DevConsole.Commands
{
    public class ApproachRateCommand : BaseCommand
    {
        public override string Command => "ar";

        public override void Execute(params string[] args)
        {
            Settings.TaikoSettings taiko = Settings.SettingsManager.SettingsStorage["Taiko"] as Settings.TaikoSettings;

            if (args == null || args.Length == 0)
            {
                DevConsole.WriteLine("Approach Rate: " + taiko.Data.ApproachRate);
                return;
            }

            if (!float.TryParse(args[0], out float newAR))
            {
                DevConsole.WriteLine($"Could not parse {args[0]} to float!");
                return;
            }

            float oldAR = taiko.Data.ApproachRate;
            taiko.Data.ApproachRate = newAR;

            DevConsole.WriteLine($"Changed Approach rate from {oldAR} to {newAR}");
        }
    }
}
