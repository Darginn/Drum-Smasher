using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DSServer.Database.Models
{
    public class Account : IEquatable<Account>
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime LastLogin { get; set; }
        public bool ResetPasswordNextLogin { get; set; }
        public long DiscordId { get; set; }
        public bool IsBanned { get; set; }
        public long PermissionLevel { get; set; }

        public Account(string accountName, string passwordHash, string passwordSalt, 
                       DateTime lastLogin, bool resetPasswordNextLogin, long discordId, 
                       bool isBanned, long permissionLevel)
        {
            AccountName = accountName;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            LastLogin = lastLogin;
            ResetPasswordNextLogin = resetPasswordNextLogin;
            DiscordId = discordId;
            IsBanned = isBanned;
            PermissionLevel = permissionLevel;
        }

        public Account()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Account);
        }

        public bool Equals([AllowNull] Account other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Account left, Account right)
        {
            return EqualityComparer<Account>.Default.Equals(left, right);
        }

        public static bool operator !=(Account left, Account right)
        {
            return !(left == right);
        }
    }
}
