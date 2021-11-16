using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;

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

            TaikoConfig taiko = (TaikoConfig)ConfigManager.GetOrLoadOrAdd<TaikoConfig>();
            
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
                    taiko.Key1 = args[1];
                    break;
                case 2:
                    taiko.Key2 = args[1];
                    break;
                case 3:
                    taiko.Key3 = args[1];
                    break;
                case 4:
                    taiko.Key4 = args[1];
                    break;
            }

            taiko.Save();

            DevConsole.WriteLine($"Updated key {keyId} to key {args[1]}");
        }
    }
}
