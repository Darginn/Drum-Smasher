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
using System.Linq;
using DSServer.Users;

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
            var acc = AccountManager.TryLogin(user, pass);

            if (acc == null)
                return false;

            Authenticated = true;
            DBId = acc.Id;
            return true;
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
            using DB db = new DB();
            var acc = db.Accounts.First(acc => acc.Id == DBId);

            Access = (AccessLevel)acc.AccessLevel;
            LoadChatUser(acc);

            if (ChatUser != null)
                IdentityManager.AddIdentity(ChatUser);
        }

        void LoadChatUser(Database.Models.Account acc)
        {
            ChatUser = new ChatUser(Id, acc.DisplayName, this);
            IdentityManager.AddIdentity(ChatUser);
        }

        public void Ban(string reason)
        {
            AccountManager.Ban(DBId);

            KickPacket kp = new KickPacket(reason, true);
            Write(kp);

            TryDisconnectAsync().ConfigureAwait(false);
        }

        public void Kick(string reason)
        {
            KickPacket kp = new KickPacket(reason, false);
            Write(kp);
            TryDisconnectAsync().ConfigureAwait(false);
        }
    }
}
