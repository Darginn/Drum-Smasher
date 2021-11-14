using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DevConsole.Commands
{
    public class KeyCommand : BaseCommand
    {
        [AutoInit]
        public static void Init()
        {
            DevConsole.Commands.Add(new KeyCommand());
        }

        public override string Command => "key";

        public override void Execute(params string[] args)
        {
            if (args.Length < 2)
            {
                DevConsole.WriteLine("key keyId keyString");
                return;
            }

            Settings.TaikoSettings taiko = Settings.SettingsManager.SettingsStorage["Taiko"] as Settings.TaikoSettings;

            if (!int.TryParse(args[0], out int keyId))
            {
                DevConsole.WriteLine($"Could not parse key id {args[0]}");
                return;
            }
            
            switch(keyId)
            {
                default:
                    keyId = 1;
                    goto case 1;

                case 1:
                    taiko.Data.Key1 = args[1];
                    break;
                case 2:
                    taiko.Data.Key2 = args[1];
                    break;
                case 3:
                    taiko.Data.Key3 = args[1];
                    break;
                case 4:
                    taiko.Data.Key4 = args[1];
                    break;
            }

            taiko.Save();

            DevConsole.WriteLine($"Updated key {keyId} to key {args[1]}");
        }
    }
}
