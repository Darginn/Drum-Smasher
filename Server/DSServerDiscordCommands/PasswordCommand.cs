using DSServer.Database;
using DSServer.Database.Models;
using DSServer.Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServerDiscordCommands
{
    [RegisterCommand]
    public class PasswordCommand : BaseCommand<CommandEventArgs>
    {
        private const string _COMMAND_NAME = "pass";

        public PasswordCommand() : base(_COMMAND_NAME)
        {

        }

        public override void OnCommand(CommandEventArgs args)
        {
            if (args.ParameterList == null || args.ParameterList.Count == 0)
            {
                args.Channel.SendMessageAsync("!pass <password>");
                return;
            }
            else if (args.Guild != null)
            {
                args.Channel.SendMessageAsync("You can only set your password via private chat!");
                return;
            }
            else if (args.Parameters.Length < 5)
            {
                args.Channel.SendMessageAsync("Your password has to be atleast 5 letters long!");
                return;
            }

            using (DSContext c = new DSContext())
            {
                Account account = c.Account.FirstOrDefault(acc => acc.DiscordId == (long)args.User.Id);

                if (account == null)
                {
                    args.Channel.SendMessageAsync("Could not find your account");
                    return;
                }

                string salt = BCrypt.Net.BCrypt.GenerateSalt(8);
                string passHash = BCrypt.Net.BCrypt.HashPassword(args.Parameters, salt);

                account.PasswordHash = passHash;
                account.PasswordSalt = salt;

                c.Account.Update(account);
                c.SaveChanges();

                args.Channel.SendMessageAsync("Successfully changed your password");
            }
        }
    }
}
