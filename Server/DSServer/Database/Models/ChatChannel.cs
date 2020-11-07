using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.Database.Models
{
    public class ChatChannel : IEquatable<ChatChannel>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public short ChatType { get; set; }
        public long Owner { get; set; }

        public ChatChannel(string name, short chatType, long owner)
        {
            Name = name;
            ChatType = chatType;
            Owner = owner;
        }

        public ChatChannel()
        {

        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChatChannel);
        }

        public bool Equals([AllowNull] ChatChannel other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(ChatChannel left, ChatChannel right)
        {
            return EqualityComparer<ChatChannel>.Default.Equals(left, right);
        }

        public static bool operator !=(ChatChannel left, ChatChannel right)
        {
            return !(left == right);
        }
    }
}
