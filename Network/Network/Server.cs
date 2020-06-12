using Network.Internal;
using Network.Packets;
using Network.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    public class Server : UdpBase
    {
        public UdpClient UdpClient => _client;
        private EncryptionKey _key;

        public byte[] GetPublicKey()
        {
            return _key.PublicKey;
        }

        /// <summary>
        /// endpoint, authenticated, last ping, encryptionkey
        /// </summary>
        private ConcurrentDictionary<IPEndPoint, User> _userCache; 

        public Server(int port) : base(IPAddress.Any, port)
        {
            _key = new EncryptionKey();
            _userCache = new ConcurrentDictionary<IPEndPoint, User>();
        }

        public Server(UdpClient client, User user) : base(client)
        {
            _key = user.EncryptionKey;
            _userCache = new ConcurrentDictionary<IPEndPoint, User>();
        }

        public Server(UdpClient client) : base(client)
        {
            _key = new EncryptionKey();
            _userCache = new ConcurrentDictionary<IPEndPoint, User>();
        }

        public Task SendPacketAsync(BasePacket packet, UdpClient user)
        {
            byte[] data = packet.ToBytes(new BinaryWriter());
            return user.SendAsync(data, data.Length);
        }

        protected override void HandleReceivedData(UdpReceiveResult result)
        {
            User user;

            if (!_userCache.TryGetValue(result.RemoteEndPoint, out user))
            {
                user = new User(result.RemoteEndPoint);
                _userCache.TryAdd(result.RemoteEndPoint, user);
            }

            PacketId packetId = (PacketId)result.Buffer[0];

            if (user.EncryptionKey == null)
            {
                if (packetId != PacketId.PublicKey)
                    return;
            }
            else if (!user.Authenticated)
            {
                if (packetId != PacketId.Authenticate)
                    return;
            }

            BasePacket bp = PacketHandler.GetPacket((PacketId)result.Buffer[0]);

            BinaryReader reader = new BinaryReader(result.Buffer);
            reader.Position++;

            bp.Read(reader, this, user);
        }
    }
}
