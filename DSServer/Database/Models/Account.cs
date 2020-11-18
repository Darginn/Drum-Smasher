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
        public byte[] PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public int AccessLevel { get; set; }
        public string DisplayName { get; set; }
        public bool IsBanned { get; set; }
        public bool IsSilenced { get; set; }
        public DateTime LastLogin { get; set; }

        public Account(string accountName, byte[] passwordHash, byte[] salt, 
                       int accessLevel, bool isBanned, string displayName,
                       bool isSilenced, DateTime lastLogin)
        {
            AccountName = accountName;
            PasswordHash = passwordHash;
            Salt = salt;
            AccessLevel = accessLevel;
            IsBanned = isBanned;
            DisplayName = displayName;
            IsSilenced = isSilenced;
            LastLogin = LastLogin;
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
