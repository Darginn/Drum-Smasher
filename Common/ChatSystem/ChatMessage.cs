using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.ChatSystem
{
    public class ChatMessage
    {
        /// <summary>
        /// Can be either a channel or user
        /// </summary>
        public ChatIdentity Sender { get; }

        /// <summary>
        /// Can be either a channel or user
        /// </summary>
        public ChatIdentity Receiver { get; }

        public string Message { get; }

        /// <param name="sender">Can be either a channel or user</param>
        /// <param name="receiver">Can be either a channel or user</param>
        public ChatMessage(ChatIdentity sender, ChatIdentity receiver, string message)
        {
            Sender = sender;
            Receiver = receiver;
            Message = message;
        }
    }
}
