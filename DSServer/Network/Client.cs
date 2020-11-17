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
            using DB db = new DB();
            var acc = db.Accounts.FirstOrDefault(acc => acc.AccountName.Equals(user, StringComparison.CurrentCultureIgnoreCase));

            if (acc == null || acc.IsBanned)
                return false;

            byte[] passBytes = Encoding.UTF8.GetBytes(pass);
            byte[] hashedPass = HashPass(ref passBytes, acc.Salt);

            Authenticated = CompareByteArrays(hashedPass, acc.PasswordHash);

            if (Authenticated)
                DBId = acc.Id;
            
            return Authenticated;
        }

        /// <summary>
        /// Hashes a password
        /// <para>
        /// Credits: https://stackoverflow.com/a/2138588 </para>
        /// </summary>
        static byte[] HashPass(ref byte[] pass, byte[] salt)
        {
            System.Security.Cryptography.HashAlgorithm algorithm = new System.Security.Cryptography.SHA256Managed();

            byte[] passHash = new byte[pass.Length + salt.Length];

            for (int i = 0; i < pass.Length; i++)
                passHash[i] = pass[i];

            for (int i = 0; i < salt.Length; i++)
                passHash[pass.Length + i] = salt[i];

            return algorithm.ComputeHash(passHash);
        }

        /// <summary>
        /// Credits: https://stackoverflow.com/a/2138588
        /// </summary>
        static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

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
            {
                IdentityManager.AddIdentity(ChatUser);

                //TODO: join default chat rooms
            }
        }

        void LoadChatUser(Database.Models.Account acc)
        {
            ChatUser = new ChatUser(Id, acc.DisplayName, this);
            IdentityManager.AddIdentity(ChatUser);
        }
    }
}
