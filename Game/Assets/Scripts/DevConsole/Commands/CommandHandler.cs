using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DevConsole.Commands
{
    public class CommandHandler
    {
        public IReadOnlyDictionary<string, ICommand> Commands => _commands;

        Dictionary<string, ICommand> _commands;

        public CommandHandler()
        {
            _commands = new Dictionary<string, ICommand>();
            RegisterDefaultCommands();
        }

        public void RegisterDefaultCommands()
        {
            RegisterCommand(new AutoplayCommand(),
                            new FPSCommand(),
                            new HelpCommand(),
                            new KeyCommand(),
                            new SongCommand());

            RegisterCommand(new SimpleActionCommand("exit", args => Application.Quit()),
                            new SimpleActionCommand("echo", args => DevConsole.Instance.WriteLine(BaseCommand.ArgsToString(args))),
                            new SimpleActionCommand("echol", args => DevConsole.Instance.Write(BaseCommand.ArgsToString(args))));

            // Clear line/s
            RegisterCommand(new SimpleActionCommand("cls", args =>
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
            }));
            
            // Log any message under "Trace" and print it to the console
            RegisterCommand(new SimpleActionCommand("log", args =>
            {
                if (args == null || args.Length == 0)
                    return;

                string logText = BaseCommand.ArgsToString(args);

                Logger.Log(logText);
                DevConsole.Instance.WriteLine($"Logged text: {logText}");
            }));
        }

        public void RegisterCommand(params ICommand[] commands)
        {
            if (commands == null)
                return;

            for (int i = 0; i < commands.Length; i++)
                RegisterCommand(commands[i]);
        }

        public void RegisterCommand(ICommand cmd)
        {
            string cmdStr = cmd.Command.ToLower();

            if (_commands.ContainsKey(cmdStr))
                throw new InvalidOperationException($"Command {cmd.Command} already registered");

            cmd.DevConsole = DevConsole.Instance;
            _commands.Add(cmdStr, cmd);
        }

        public void TryParseLineToCommand(string line)
        {
            string[] split = line.Split(' ');

            if (split == null || split.Length == 0)
                return;

            string[] parameters = new string[split.Length - 1];

            for (int i = 1; i < split.Length; i++)
                parameters[i - 1] = split[i];

            TryExecuteCommand(split[0], parameters);
        }

        public bool TryExecuteCommand(string cmd, string[] args)
        {
            if (!_commands.TryGetValue(cmd.ToLower(), out ICommand command))
            {
                DevConsole.Instance.WriteLine($"Command {cmd} not found");
                return false;
            }
            else if (args == null)
                args = Array.Empty<string>();


            try
            {
                command.Execute(args);
            }
            catch (Exception ex)
            {
                Color oldCol = DevConsole.Instance.GetFontColor();

                DevConsole.Instance.SetFontColor(Color.red);
                DevConsole.Instance.WriteLine($"Failed to execute command: {ex}");
                DevConsole.Instance.SetFontColor(oldCol);

                Logger.Log(ex.ToString(), LogLevel.Error);
            }

            return true;
        }
    }
}
