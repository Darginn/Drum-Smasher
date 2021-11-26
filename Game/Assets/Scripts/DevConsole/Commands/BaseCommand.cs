using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DevConsole.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public DevConsole DevConsole { get; set; }

        public abstract string Command { get; }

        public abstract void Execute(params string[] args);


        public static string ArgsToString(string[] args, int start = 0)
        {
            if (args == null || args.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            for (int i = start; i < args.Length; i++)
                sb.Append($"{args[i]} ");

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}
