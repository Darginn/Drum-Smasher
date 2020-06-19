using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon
{
    public class AccountData : IEquatable<AccountData>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as AccountData);
        }

        public bool Equals(AccountData other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(AccountData left, AccountData right)
        {
            return EqualityComparer<AccountData>.Default.Equals(left, right);
        }

        public static bool operator !=(AccountData left, AccountData right)
        {
            return !(left == right);
        }
    }
}
