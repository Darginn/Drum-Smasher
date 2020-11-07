using DSServer.Database;
using DSServer.Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServerDiscordCommands
{
    [RegisterCommand]
    public class ReloadCommandsCommand : BaseCommand<CommandEventArgs>
    {
        private const string _COMMAND_NAME = "reload";
        public ReloadCommandsCommand() : base(_COMMAND_NAME)
        {

        }

        public override void OnCommand(CommandEventArgs args)
        {
            using (DSContext c = new DSContext())
            {
                var acc = c.Account.FirstOrDefault(ac => ac.DiscordId == (long)args.User.Id);

                if (acc == null || acc.PermissionLevel != 1 && acc.PermissionLevel != 2)
                {
                    args.Channel.SendMessageAsync("You do not have enough permissions to use this command!");
                    return;
                }
            }

                System.Threading.Tasks.Task.Run(() => args.ChatClient.InitializeCommandHandler());
        }
    }
}
