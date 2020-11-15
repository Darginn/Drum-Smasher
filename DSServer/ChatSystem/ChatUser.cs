using DSServer.Network;
using DSServerCommon.ChatSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.ChatSystem
{
    public class ChatUser : ChatIdentity
    {
        public Client Client { get; }

        public ChatUser(Guid id, string name, Client client) : base(id, name)
        {
            Client = client;
        }

        /// <summary>
        /// Leave a specific chat
        /// </summary>
        public void ExitChat(ChatIdentity chat)
        {
            chat.OnChatExit(this, null);
        }

        /// <summary>
        /// Join a specific chat
        /// </summary>
        public void JoinChat(ChatIdentity chat)
        {
            chat.OnChatJoin(this, null);
        }

        /// <summary>
        /// Sends a message to <paramref name="dest"/>
        /// </summary>
        public void SendMessageTo(ChatIdentity dest, string message)
        {
            dest.OnChatMessage(new ChatMessage(this, dest, message));
        }

        //TODO: ChatUser network part
        public override void OnChatExit(ChatIdentity user, ChatIdentity chat)
        {
            //Send data to client
        }

        public override void OnChatJoin(ChatIdentity user, ChatIdentity chat)
        {
            //Send data to client
        }

        public override void OnChatMessage(ChatMessage message)
        {
            //Send data to client
        }
    }
}
