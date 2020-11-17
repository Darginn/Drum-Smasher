using DSServer.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSServer.Network.Packets;
using DSServerCommon.Network.Packets;
using System.Diagnostics;

namespace DSServer.Network
{
    public class Client : NetState
    {
        public bool Authenticated { get; private set; }
        public ChatUser ChatUser { get; private set; }
        public AccessLevel Access { get; private set; }
        public long DBId { get; private set; }

        Stopwatch _authWatch;
        const double _AUTH_TIMEOUT_MS = 60000; //60 seconds 

        public Client(System.Net.Sockets.Socket socket) : base(socket)
        {
            OnStarted += RequestAuthentication;
            OnDisconnected += OnDisconnect;
        }

        public bool Authenticate(string user, string pass)
        {
            //TODO: do authentication
            Authenticated = false;

            return Authenticated;
        }

        private void OnDisconnect(object sender, Guid e)
        {
            List<ChatRoom> rooms = IdentityManager.GetAllChatRooms();

            for (int i = 0; i < rooms.Count; i++)
                rooms[i].OnChatExit(ChatUser);

            IdentityManager.RemoveIdentity(ChatUser);
        }

        /// <summary>
        /// Requests authentication from the client
        /// <para>If not authenticated within <see cref="_AUTH_TIMEOUT_MS"/> client will be disconnected</para>
        /// </summary>
        void RequestAuthentication(NetState state)
        {
            Task.Run(() =>
            {
                _authWatch.Start();

                while (!base.UseEncryption)
                {
                    if (!base.Connected)
                        return;
                    else if (_authWatch.ElapsedMilliseconds >= _AUTH_TIMEOUT_MS)
                    {
                        TryDisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        return;
                    }

                    Task.Delay(50).ConfigureAwait(false).GetAwaiter().GetResult();
                }

                RequestCredentsPacket rcp = new RequestCredentsPacket(Id);
                Write(rcp);

                while (!Authenticated)
                {
                    if (!base.Connected)
                        return;
                    else if (_authWatch.ElapsedMilliseconds >= _AUTH_TIMEOUT_MS)
                    {
                        TryDisconnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                        return;
                    }

                    Task.Delay(50).ConfigureAwait(false).GetAwaiter().GetResult();
                }

                _authWatch.Stop();

                if (!base.Connected)
                    return;

                LoadClient(state);
            }).ConfigureAwait(false);
        }

        protected override void ExecutePacket(int packetId, Packet packet)
        {
            //Only accept RequestCredents packet if we are not authenticated
            if (!Authenticated && packetId != (int)PacketId.RequestCredents)
                return;

            base.ExecutePacket(packetId, packet);
        }

        void LoadClient(NetState state)
        {
            //TODO: load access level from db

            Access = AccessLevel.User;
            LoadChatUser();

            if (ChatUser != null)
            {
                IdentityManager.AddIdentity(ChatUser);

                //TODO: join default chat rooms
            }
        }

        void LoadChatUser()
        {
            //TODO: load chat identity + db id from db
            DBId = 0;
            ChatUser = new ChatUser(Id, "", this);
            IdentityManager.AddIdentity(ChatUser);
        }
    }
}
