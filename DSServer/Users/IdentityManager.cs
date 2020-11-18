using DSServer.ChatSystem;
using DSServerCommon.ChatSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServer.Users
{
    public static class IdentityManager
    {
        static Dictionary<Guid, ChatIdentity> _identities = new Dictionary<Guid, ChatIdentity>();
        static readonly object _idLock = new object();

        public static bool TryGetChatRoom(Guid id, out ChatRoom room)
        {
            if (TryGetIdentity(id, out ChatIdentity identity))
            {
                room = identity as ChatRoom;
                return room != null;
            }

            room = null;
            return false;
        }

        public static void AddIdentity(ChatIdentity identity)
        {
            lock(_idLock)
            {
                if (_identities.ContainsKey(identity))
                    return;

                _identities.Add(identity, identity);
            }
        }

        public static void RemoveIdentity(ChatIdentity identity)
        {
            lock(_idLock)
            {
                _identities.Remove(identity);
            }
        }

        public static bool TryGetIdentity(Guid id, out ChatIdentity identity)
        {
            lock(_idLock)
            {
                if (!_identities.ContainsKey(id))
                {
                    identity = null;
                    return false;
                }

                identity = _identities[id];
                return identity != null;
            }
        }

        public static bool TryGetChatUser(Guid id, out ChatUser user)
        {
            if (TryGetIdentity(id, out ChatIdentity identity))
            {
                user = identity as ChatUser;
                return user != null;
            }

            user = null;
            return false;
        }

        public static List<ChatRoom> GetAllChatRooms()
        {
            List<ChatRoom> result = new List<ChatRoom>();

            lock(_idLock)
            {
                foreach(var id in _identities.Values)
                {
                    ChatRoom room = id as ChatRoom;

                    if (room != null)
                        result.Add(room);
                }
            }

            return result;
        }

        public static List<ChatUser> GetAllUsers()
        {
            List<ChatUser> result = new List<ChatUser>();

            lock(_idLock)
            {
                foreach(var id in _identities.Values)
                {
                    ChatUser user = id as ChatUser;

                    if (user != null)
                        result.Add(user);
                }
            }

            return result;
        }

        public static List<ChatIdentity> GetAllIdentities()
        {
            lock(_idLock)
            {
                return _identities.Values.ToList();
            }
        }
    }
}
