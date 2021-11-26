using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DevConsole.Commands
{
    public static class Commands
    {
        public static void RegisterDefaultCommands(CommandHandler handler)
        {
            handler.RegisterCommand(new AutoplayCommand(),
                                    new FPSCommand(),
                                    new HelpCommand(),
                                    new KeyCommand(),
                                    new SongCommand());

            handler.RegisterCommand(new SimpleActionCommand("exit", ExitCommand),
                                    new SimpleActionCommand("echo", EchoCommand),
                                    new SimpleActionCommand("echol", EchoLCommand),
                                    new SimpleActionCommand("cls", CLSCommand),
                                    new SimpleActionCommand("log", LogCommand));
        }

        #region cmd impl
        static void ExitCommand(string[] args)
        {
            Application.Quit();
        }

        static void EchoCommand(string[] args)
        {
            DevConsole.Instance.WriteLine(BaseCommand.ArgsToString(args));
        }

        static void EchoLCommand(string[] args)
        {
            DevConsole.Instance.Write(BaseCommand.ArgsToString(args));
        }

        static void CLSCommand(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    default:
                        DevConsole.Instance.ClearAll();
                        break;

                    case "oldest":
                    case "first":
                    case "f":
                        DevConsole.Instance.Clear(order: DevConsole.ClearOrder.NewestToOldest);
                        break;

                    case "newest":
                    case "last":
                    case "l":
                        DevConsole.Instance.Clear(order: DevConsole.ClearOrder.OldestToNewest);
                        break;
                }
            }
            else
                DevConsole.Instance.ClearAll();
        }

        static void LogCommand(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            string logText = BaseCommand.ArgsToString(args);

            Logger.Log(logText);
            DevConsole.Instance.WriteLine($"Logged text: {logText}");
        }
        #endregion
    }
}
