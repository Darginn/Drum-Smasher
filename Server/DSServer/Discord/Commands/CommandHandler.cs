using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DSServer.Discord.Commands
{
    public class CommandHandler<T>
    {
        public ConcurrentDictionary<string, BaseCommand<T>> Commands { get; private set; }
        private char _commandTrigger;

        public CommandHandler(char commandTrigger)
        {
            _commandTrigger = commandTrigger;
            Commands = new ConcurrentDictionary<string, BaseCommand<T>>();
        }

        public void LoadCommands(Assembly ass)
        {
            Console.WriteLine($"Loading commands from {ass.FullName}");

            Type[] types = ass.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                RegisterCommandAttribute regCmdAttrib = types[i].GetCustomAttribute<RegisterCommandAttribute>();

                if (regCmdAttrib == null)
                    continue;

                try
                {
                    BaseCommand<T> cmd = Activator.CreateInstance(types[i]) as BaseCommand<T>;

                    if (cmd == null)
                    {
                        Console.WriteLine($"Could not create command instance of {types[i].Name}");
                        continue;
                    }

                    cmd.Initialize();

                    if (!Commands.TryAdd(cmd.Command.ToLower(), cmd))
                    {
                        Console.WriteLine($"Could not register command {cmd.Command}");
                        cmd.Dispose();
                        continue;
                    }

                    Console.WriteLine($"Registered command {cmd.Command}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occured loading command type {types[i].Name}:\n{ex.ToString()}");
                }
            }

            Console.WriteLine("Loaded commands");
        }

        public void UnloadCommands()
        {
            Console.WriteLine("Unloading commands");

            List<BaseCommand<T>> baseCommands = Commands.Values.ToList();
            Commands.Clear();

            for (int i = 0; i < baseCommands.Count; i++)
            {
                try
                {
                    baseCommands[i].Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to unload command {baseCommands[i].Command}");
                }
            }

            Console.WriteLine("Commands unloaded");
        }

        public bool InvokeCommand(string command, T args)
        {
            command = command.TrimStart(_commandTrigger);

            if (!Commands.TryGetValue(command.ToLower(), out BaseCommand<T> cmd))
                return false;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    cmd.OnCommand(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            return true;
        }
    }
}
