using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.ChatSystem
{
    /// <summary>
    /// A chat identity, can either be a user or channel
    /// </summary>
    public class ChatIdentity
    {
        //TODO: find a custom name for Chat System Identity
        public static ChatIdentity System => new ChatIdentity("Rin", true);

        public Guid Id { get; private set; }
        public string Name { get; protected set; }
        public bool IsSystemIdentity { get; private set; }

        /// <summary>
        /// A chat identity, can either be a user or channel
        /// </summary>
        public ChatIdentity(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// A chat identity, can either be a user or channel
        /// </summary>
        public ChatIdentity(string name) : this(Guid.NewGuid(), name)
        {

        }

        /// <summary>
        /// A chat identity, can either be a user or channel
        /// </summary>
        private ChatIdentity(string name, bool isSystemIdentity) : this(name)
        {
            IsSystemIdentity = isSystemIdentity;
        }

        /// <summary>
        /// A message has been sent to this identity
        /// </summary>
        /// <param name="message"></param>
        public virtual void OnChatMessage(ChatMessage message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An entity joined a chat
        /// </summary>
        public virtual void OnChatJoin(ChatIdentity user, ChatIdentity chat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// An identity left a chat
        /// </summary>
        public virtual void OnChatExit(ChatIdentity user, ChatIdentity chat)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a message to this identity
        /// </summary>
        public void SendMessage(ChatIdentity sender, string message)
        {
            OnChatMessage(new ChatMessage(sender, this, message));
        }

        public static implicit operator Guid(ChatIdentity identity)
        {
            return identity.Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
