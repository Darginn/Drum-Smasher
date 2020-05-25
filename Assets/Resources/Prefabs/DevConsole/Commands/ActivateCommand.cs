using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Prefab.DevConsole.Commands
{
    public class ActivateCommand : BaseCommand
    {
        [AutoInit]
        public static void Init()
        {
            DevConsole.Commands.Add(new ActivateCommand());
        }

        public override string Command => "ac";

        public override void Execute(params string[] args)
        {
            if (args == null || args.Length < 2)
            {
                DevConsole.WriteLine("ac mod true/false");
                return;
            }

            GameObject gobj = GameObject.Find(args[0]);

            bool state = false;
            bool.TryParse(args[1], out state);

            BoxCollider2D boxCollider2D = gobj.GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = state;
        }
    }
}

