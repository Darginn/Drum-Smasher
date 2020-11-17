using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DSServer.Database.Models
{
    public class Warning : IEquatable<Warning>
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Reason { get; set; }

        public Warning(long accountId, string reason)
        {
            AccountId = accountId;
            Reason = reason;
        }

        public Warning()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Warning);
        }

        public bool Equals([AllowNull] Warning other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Warning left, Warning right)
        {
            return EqualityComparer<Warning>.Default.Equals(left, right);
        }

        public static bool operator !=(Warning left, Warning right)
        {
            return !(left == right);
        }
    }
}
