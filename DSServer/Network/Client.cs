using DSServer.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Network
{
    public class Client : NetState
    {
        public ChatUser ChatUser { get; private set; }
        public AccessLevel Access { get; private set; }
        public long DBId { get; private set; }

        public Client(System.Net.Sockets.Socket socket) : base(socket)
        {
            OnStarted += LoadClient;
            OnDisconnected += OnDisconnect;
        }

        private void OnDisconnect(object sender, Guid e)
        {
            //TODO: signal chatrooms that user exited
            Access = AccessLevel.User;
        }

        void LoadClient(NetState state)
        {
            //TODO: load access level from db

            LoadChatUser();
        }

        void LoadChatUser()
        {
            //TODO: load chat identity + db id from db
            DBId = 0;
            ChatUser = new ChatUser(Id, "", this);
        }
    }
}
