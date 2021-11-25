using Assets.Scripts.TaikoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DevConsole.Commands
{
    public class SongCommand : BaseCommand
    {
        public override string Command => "Song";

        [AutoInit]
        public static void Init()
        {
            DevConsole.Commands.Add(new SongCommand());
        }

        public override void Execute(params string[] args)
        {
            if (args.Length == 0)
            {
                DevConsole.WriteLine("Parameters missing, possible parameters: Restart, Resume, Pause");
                return;
            }

            switch(args[0].ToLower())
            {
                case "restart":
                    NoteScroller.Instance.ReloadScene();
                    break;

                case "resume":
                    SoundConductor.Instance.Resume();
                    break;

                case "pause":
                    SoundConductor.Instance.Pause();
                    break;

                default:
                    DevConsole.WriteLine($"Parameter {args[0]} not found!");
                    break;
            }
        }
    }
}
