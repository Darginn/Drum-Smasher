using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DSServer.Discord.Commands;

namespace DSServer.Discord
{
    public class DiscordChatClient
    {
        public DiscordClient Client => _discordClient;
        private DiscordClient _discordClient;
        private CommandHandler<CommandEventArgs> _commandHandler;

        private const string _DISCORD_COMMAND_LIBRARY = "DSServerDiscordCommands.dll";
        private const char _COMMAND_TRIGGER = '!';
        
        public DiscordChatClient()
        {
            Console.WriteLine("Loading Discord");

            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("DiscordSecret", EnvironmentVariableTarget.Process),
                TokenType = TokenType.Bot,
                AutoReconnect = true
            });

            _discordClient.MessageCreated += async args => OnMessageReceived(args);
            _discordClient.Ready += async args => Console.WriteLine("DiscordClient ready");
            _discordClient.ClientErrored += async args => Console.WriteLine($"Client Error:\n{args.Exception}");
            _discordClient.SocketErrored += async args => Console.WriteLine($"Socket Error:\n{args.Exception}");

            InitializeCommandHandler();

            Console.WriteLine("Discord Loaded");
        }

        public void InitializeCommandHandler()
        {
            _commandHandler?.UnloadCommands();

            _commandHandler = new CommandHandler<CommandEventArgs>(_COMMAND_TRIGGER);

            if (File.Exists(_DISCORD_COMMAND_LIBRARY))
                _commandHandler.LoadCommands(System.Reflection.Assembly.LoadFrom(_DISCORD_COMMAND_LIBRARY));
            else
                Console.WriteLine($"Discord command library not found, there will be no commands available!");
        }

        public async Task ConnectAsync()
        {
            await _discordClient.ConnectAsync();
        }

        private void OnMessageReceived(MessageCreateEventArgs args)
        {
            if (args.Author.Id == _discordClient.CurrentUser.Id ||
                string.IsNullOrEmpty(args.Message.Content))
                return;

            Console.WriteLine($"{DateTime.UtcNow}: New message from {args.Author.Username}: {args.Message.Content}");

            List<string> parameterList = args.Message.Content.Split(' ').ToList();
            string command = parameterList[0].TrimStart(_COMMAND_TRIGGER);
            string parameters = null;

            if (parameterList.Count > 1)
            {
                parameters = args.Message.Content.Remove(0, parameterList[0].Length + 1);
                parameterList.RemoveAt(0);
            }
            else
            {
                parameterList = new List<string>();
                parameters = null;
            }

            CommandEventArgs commandEventArgs = new CommandEventArgs(this, _discordClient, args.Guild, args.Author, args.Channel, args.Message, parameters, parameterList);
            _commandHandler.InvokeCommand(command, commandEventArgs);
        }
    }
}
