using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Discord.Commands
{
    public class CommandEventArgs : EventArgs
    {
        public DiscordChatClient ChatClient { get; }
        public DiscordClient Client { get; }
        public DiscordGuild Guild { get; }
        public DiscordUser User { get; }
        public DiscordChannel Channel { get; }
        public DiscordMessage Message { get; }
        public string Parameters { get; }
        public List<string> ParameterList { get; }

        public CommandEventArgs(DiscordChatClient discordChatClient, DiscordClient client, DiscordGuild guild, DiscordUser user, DiscordChannel channel, DiscordMessage message, string parameters, List<string> parameterList) 
        {
            ChatClient = discordChatClient;
            Client = client;
            Guild = guild;
            User = user;
            Channel = channel;
            Message = message;
            Parameters = parameters;

            if (parameterList == null)
                ParameterList = new List<string>();
            else
                ParameterList = parameterList;
        }
    }
}
