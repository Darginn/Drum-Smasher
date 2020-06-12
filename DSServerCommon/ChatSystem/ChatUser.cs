using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.ChatSystem
{
    public class ChatUser : IEquatable<ChatUser>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float[] RGBChatNameColor { get; set; }

        public ChatUser(long id, string name, float[] rgbChatNameColor)
        {
            Id = id;
            Name = name;
            RGBChatNameColor = rgbChatNameColor;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChatUser);
        }

        public bool Equals(ChatUser other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(ChatUser left, ChatUser right)
        {
            return EqualityComparer<ChatUser>.Default.Equals(left, right);
        }

        public static bool operator !=(ChatUser left, ChatUser right)
        {
            return !(left == right);
        }
    }
}
