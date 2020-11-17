using DSServerCommon.ChatSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.ChatSystem
{
    public class ChatRoom : ChatIdentity
    {
        Dictionary<Guid, ChatIdentity> _users;
        readonly object _usersLock = new object();

        public ChatRoom(string name) : base(name)
        {
            _users = new Dictionary<Guid, ChatIdentity>();
        }

        public static void InitializeChatRooms()
        {
            ChatRoom defaultRoom = new ChatRoom("#General");
            IdentityManager.AddIdentity(defaultRoom);
        }

        /// <summary>
        /// A message has been sent to this channel
        /// </summary>
        public override void OnChatMessage(ChatMessage message)
        {
            lock(_usersLock)
            {
                foreach (var user in _users.Values)
                    user.OnChatMessage(message);
            }
        }

        /// <summary>
        /// User joined the chat
        /// </summary>
        public void OnChatJoin(ChatIdentity identity)
        {
            lock(_usersLock)
            {
                if (_users.ContainsKey(identity))
                    return;

                foreach (var user in _users.Values)
                    user.OnChatJoin(identity, this);

                _ = _users.TryAdd(identity, identity);
            }
        }

        /// <summary>
        /// User left the chat
        /// </summary>
        public void OnChatExit(ChatIdentity identity)
        {
            lock (_usersLock)
            {
                if (!_users.ContainsKey(identity))
                    return;

                _ = _users.Remove(identity);

                foreach (var user in _users.Values)
                    user.OnChatExit(identity, this);
            }
        }

        /// <summary>
        /// Sends a <paramref name="message"/> through the <see cref="ChatIdentity.System"/> identity
        /// </summary>
        public void SystemMessage(string message)
        {
            SendMessage(ChatIdentity.System, message);
        }

        /// <summary>
        /// Sends a <paramref name="message"/> while impersonating <paramref name="sender"/>
        /// </summary>
        public void FakeMessage(ChatIdentity sender, string message)
        {
            SendMessage(sender, message);
        }

        /// <summary>
        /// Calls <see cref="OnChatExit(ChatIdentity)"/>
        /// </summary>
        public override void OnChatExit(ChatIdentity user, ChatIdentity chat)
        {
            OnChatExit(user);
        }

        /// <summary>
        /// Calls <see cref="OnChatJoin(ChatIdentity)"/>
        /// </summary>
        public override void OnChatJoin(ChatIdentity user, ChatIdentity chat)
        {
            OnChatJoin(user);
        }
    }
}
