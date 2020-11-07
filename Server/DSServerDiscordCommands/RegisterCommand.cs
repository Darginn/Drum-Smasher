using DSServer.Database;
using DSServer.Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServerDiscordCommands
{
    [RegisterCommand]
    public class RegisterCommand : BaseCommand<CommandEventArgs>
    {
        private const string _COMMAND_NAME = "register";

        public RegisterCommand() : base(_COMMAND_NAME)
        {

        }

        public override void OnCommand(CommandEventArgs args)
        {
            if (args.ParameterList == null || args.ParameterList.Count == 0)
            {
                args.Channel.SendMessageAsync("!register <username with spaces>");
                return;
            }
            else if (args.Guild != null)
            {
                args.Channel.SendMessageAsync("You can only register via private chat!");
                return;
            }

            string username = args.Parameters;

            using (DSContext c = new DSContext())
            {
                if (c.Account.Any(acc => acc.AccountName.Equals(username, StringComparison.CurrentCultureIgnoreCase)))
                {
                    args.Channel.SendMessageAsync("This username already exists");
                    return;
                }

                c.Account.Add(new DSServer.Database.Models.Account(username, ".", ".", DateTime.MinValue, false, (long)args.User.Id, false, 0));
                c.SaveChanges();

                args.Channel.SendMessageAsync("Created your account, use !pass <password> to set your password");
            }
        }
    }
}
