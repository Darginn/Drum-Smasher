using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.ChatSystem
{
    public class ChatMessage : IEquatable<ChatMessage>
    {
        public long UserId { get; set; }
        public long Destination { get; set; }
        public bool IsChannel { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage(long userId, long destination, bool isChannel, string message)
        {
            UserId = userId;
            Destination = destination;
            IsChannel = isChannel;
            Message = message;

            Timestamp = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChatMessage);
        }

        public bool Equals(ChatMessage other)
        {
            return other != null &&
                   UserId == other.UserId &&
                   Destination == other.Destination &&
                   IsChannel == other.IsChannel &&
                   Message == other.Message &&
                   Timestamp == other.Timestamp;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, Destination, IsChannel, Message, Timestamp);
        }

        public static bool operator ==(ChatMessage left, ChatMessage right)
        {
            return EqualityComparer<ChatMessage>.Default.Equals(left, right);
        }

        public static bool operator !=(ChatMessage left, ChatMessage right)
        {
            return !(left == right);
        }
    }
}
