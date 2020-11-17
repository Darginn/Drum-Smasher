using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.Discord
{
    public static class DClient
    {
        public const char INVISIBLE_CHAR = '‎';
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

            LoadCommands();

            _client.ConnectAsync().ConfigureAwait(false);
        }

        static void LoadCommands()
        {

        }

        /// <summary>
        /// Sends a simple embed to a channel
        /// </summary>
        public static async Task<DiscordMessage> SendSimpleEmbedAsync(DiscordChannel channel, string title, string message = null)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = message
            };

            return await channel.SendMessageAsync(embed: builder.Build()).ConfigureAwait(false);
        }
        public static async Task<DiscordMessage> SendSimpleEmbedAsync(ulong channelId, string title, string message = null)
        {
            DiscordChannel channel = await GetChannelAsync(channelId).ConfigureAwait(false);

            if (channel == null)
                return null;

            return await SendSimpleEmbedAsync(channel, title, message).ConfigureAwait(false);
        }
        public static async Task<DiscordMessage> SendSimpleEmbedAsync(DiscordGuild guild, ulong channelId, string title, string message = null)
        {
            DiscordChannel channel = GetChannel(guild, channelId);

            if (channel == null)
                return null;

            return await SendSimpleEmbedAsync(channel, title, message).ConfigureAwait(false);
        }
        public static async Task<DiscordMessage> SendSimpleEmbedAsync(ulong guildId, ulong channelId, string title, string message = null)
        {
            DiscordGuild guild = await GetGuildAsync(guildId).ConfigureAwait(false);

            if (guild == null)
                return null;

            return await SendSimpleEmbedAsync(guild, channelId, title, message);
        }

        public static async Task<DiscordGuild> GetGuildAsync(ulong id)
        {
            try
            {
                return await _client.GetGuildAsync(id).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<DiscordDmChannel> GetChannelAsync(DiscordGuild guild, DiscordUser user)
        {
            try
            {
                DiscordMember member = await guild.GetMemberAsync(user.Id).ConfigureAwait(false);
                return await member.CreateDmChannelAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<DiscordDmChannel> GetChannelAsync(DiscordGuild guild, ulong userId)
        {
            DiscordMember member = await GetMemberAsync(guild, userId).ConfigureAwait(false);

            if (member == null)
                return null;

            return await GetChannelAsync(member).ConfigureAwait(false);
        }
        public static async Task<DiscordDmChannel> GetChannelAsync(ulong guildId, ulong userId)
        {
            DiscordGuild guild = await GetGuildAsync(guildId).ConfigureAwait(false);

            if (guild == null)
                return null;

            return await GetChannelAsync(guild, userId).ConfigureAwait(false);
        }

        public static async Task<DiscordDmChannel> GetChannelAsync(DiscordMember member)
        {
            try
            {
                return await member.CreateDmChannelAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<DiscordChannel> GetChannelAsync(ulong id)
        {
            try
            {
                return await _client.GetChannelAsync(id).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DiscordChannel GetChannel(DiscordGuild guild, ulong id)
        {
            try
            {
                return guild.GetChannel(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<DiscordUser> GetUserAsync(ulong id)
        {
            try
            {
                return await _client.GetUserAsync(id).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<DiscordMember> GetMemberAsync(DiscordGuild guild, ulong id)
        {
            try
            {
                return await guild.GetMemberAsync(id).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<DiscordMember> GetMemberAsync(ulong guildId, ulong id)
        {
            DiscordGuild guild = await GetGuildAsync(guildId).ConfigureAwait(false);

            if (guild == null)
                return null;

            return await GetMemberAsync(guild, id);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            //Ignore our own messages
            if (e.Author.Id == _client.CurrentUser.Id)
                return;
        }

        static async Task OnReady(ReadyEventArgs e)
        {
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

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
