using DSServer.Network;
using DSServer.Network.Packets;
using DSServerCommon.ChatSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServer.ChatSystem
{
    public class ChatUser : ChatIdentity
    {
        public Client Client { get; }
        public bool IsSilenced { get; private set; }

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

        public override void OnChatExit(ChatIdentity user, ChatIdentity chat)
        {
            ChatExitPacket cep = new ChatExitPacket(user, chat);
            Client.Write(cep);
        }

        public override void OnChatJoin(ChatIdentity user, ChatIdentity chat)
        {
            ChatJoinPacket cjp = new ChatJoinPacket(user, chat);
            Client.Write(cjp);
        }

        public override void OnChatMessage(ChatMessage message)
        {
            ChatMessagePacket cmp = new ChatMessagePacket(message.Sender, message.Receiver, message.Message);
            Client.Write(cmp);
        }

        public void Silence()
        {
            IsSilenced = true;
            
            using DB db = new DB();
            var acc = db.Accounts.First(acc => acc.Id == Client.DBId);

            acc.IsSilenced = true;

            db.Accounts.Update(acc);
            db.SaveChanges();
        }

        public void DeSilence()
        {
            IsSilenced = false;

            using DB db = new DB();
            var acc = db.Accounts.First(acc => acc.Id == Client.DBId);

            acc.IsSilenced = false;

            db.Accounts.Update(acc);
            db.SaveChanges();
        }
    }
}
