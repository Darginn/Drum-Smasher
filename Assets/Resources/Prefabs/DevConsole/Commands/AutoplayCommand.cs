using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Prefab.DevConsole.Commands
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
            Settings.TaikoSettings taiko = Settings.SettingsManager.SettingsStorage["Taiko"] as Settings.TaikoSettings;

            if (args == null || args.Length == 0)
            {
                DevConsole.WriteLine("Autoplay: " + taiko.Data.Autoplay);
                return;
            }

            if (!bool.TryParse(args[0], out bool newAP))
            {
                DevConsole.WriteLine($"Could not parse {args[0]} to float!");
                return;
            }

            bool oldAP = taiko.Data.Autoplay;
            taiko.Data.Autoplay = newAP;

            taiko.Save();

            DevConsole.WriteLine($"Changed Autoplay from {oldAP} to {newAP}");
        }
    }
}
