using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.Discord
{
    public static class DClient
    {
        const string _DSECRET_FILE = "discord.secret";

        static DiscordClient _client;

        public static void InitializeDiscord()
        {
            string secret = LoadSecret(_DSECRET_FILE);
            _client = new DiscordClient(new DiscordConfiguration()
            {
                Token = secret,
                TokenType = TokenType.Bearer,
                AutoReconnect = true
            });

            _client.Ready += (s, e) => Task.Run(async () => await OnReady(e));
            _client.MessageCreated += (s, e) => Task.Run(async () => await OnMessageCreated(e));
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            //Ignore our own messages
            if (e.Author.Id == _client.CurrentUser.Id)
                return;
        }

        private static async Task OnReady(ReadyEventArgs e)
        {
        }

        static string LoadSecret(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException(nameof(file));
            else if (!System.IO.File.Exists(file))
                throw new System.IO.FileNotFoundException("File not found", file);

            return System.IO.File.ReadAllText(file);
        }
    }
}
