using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.ChatSystem
{
    public class ChatChannel
    {
        public long Id;
        public string Name;

        protected List<ChatUser> _users;
        protected ICollection _userCollection;
        
        public ChatChannel(long id, string name)
        {
            Id = id;
            Name = name;
            _users = new List<ChatUser>();
            _userCollection = (ICollection)_users;
        }

        public void OnUserJoin(ChatUser user)
        {
            lock(_userCollection.SyncRoot)
            {
                if (_users.Contains(user))
                {
                    OnUserJoined(user, false);
                    return;
                }

                _users.Add(user);
            }

            OnUserJoined(user, true);
        }

        public virtual void OnUserJoined(ChatUser user, bool success)
        {

        }

        public void OnUserLeave(ChatUser user)
        {
            lock (_userCollection.SyncRoot)
            {
                if (!_users.Contains(user))
                {
                    OnUserLeaved(user, false);
                    return;
                }

                _users.Remove(user);
            }

            OnUserLeaved(user, true);
        }

        public virtual void OnUserLeaved(ChatUser user, bool success)
        {

        }

        public virtual void OnChatMessageReceived(ChatUser user, ChatMessage message)
        {

        }
    }
}
