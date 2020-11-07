using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DSServer.Database.Models;
using DSServerCommon;
using DSServerCommon.Packets;
using DSServerCommon.Encryption;
using NetCoreServer;
using DSServer.Server.Packets;
using DSServerCommon.ChatSystem;
using System.Linq;

namespace DSServer
{
    public class DSSession : TcpSession
    {
        public new DSServer Server { get; private set; }
        public IPEndPoint IPEndPoint;
        /// <summary>
        /// Encryption key for data transmission
        /// </summary>
        public RijndaelEncryption Encryption { get; set; }
        /// <summary>
        /// Packethandler
        /// </summary>
        public PacketHandler<DSSession> PacketHandler { get; private set; }

        public ChatUser ChatUser;
        public AccountData AccountData { get; private set; }
        /// <summary>
        /// Is user authenticated
        /// </summary>
        public bool IsAuthenticated => _isAuthenticated && AccountData != null;

        private bool _isAuthenticated;
        private ILogger _logger;

        public DSSession(DSServer server, PacketHandler<DSSession> packetHandler, ILogger logger) : base(server)
        {
            Server = server;
            _logger = logger;
            PacketHandler = packetHandler;
        }

        public void OnAuthenticated(Account account)
        {
            AccountData = new AccountData()
            {
                Id = account.Id,
                Name = account.AccountName,
                IsAdmin = account.PermissionLevel == 1 || account.PermissionLevel == 2
            };

            _isAuthenticated = true;

            

            List<AccountData> accounts = Server.Sessions.Values.Where(s => s.IsAuthenticated && s.AccountData.Id != AccountData.Id)
                                                               .Select(s => s.AccountData)
                                                               .ToList();

            int i = 0;
            while (i < accounts.Count)
            {
                int length = i + 10 >= accounts.Count ? accounts.Count - i : 10;

                List<AccountData> accRange = accounts.GetRange(i, length);

                i += length;

                UserDataPacket rudp = new UserDataPacket(_logger, accRange.ToArray());
                PacketWriter writer = rudp.WriteData(new PacketWriter());
                rudp.InsertPacketId(ref writer);

                SendData(writer.ToBytes());
            }
            
            //TODO: notify authenticated clients about new user login
        }

        public void TryJoinChat(long channelId)
        {
            Program.MainChat.OnUserJoin(ChatUser);
        }

        public void TryPartChat(long channelId)
        {
            Program.MainChat.OnUserLeave(ChatUser);
        }

        public void TrySendMessage(DSServerCommon.ChatSystem.ChatMessage cmsg)
        {
            Program.MainChat.OnChatMessageReceived(ChatUser, cmsg);
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            base.OnReceived(buffer, offset, size);
            Console.WriteLine($"Received {size} bytes");

            byte[] data;
            if (size != buffer.Length)
            {
                data = new byte[size];
                Array.Copy(buffer, offset, data, 0, size);
            }
            else
                data = buffer;

            HandleData(ref data);
        }

        private void HandleData(ref byte[] data)
        {
            try
            {
                //If encryption available, decrypt data
                if (Encryption != null && Encryption.Key != null && Encryption.IV != null)
                    data = Encryption.Decrypt(data);

                //Get packet id
                short pkId = BitConverter.ToInt16(data, 0);
                PacketId? pId;

                //Check if packet id is valid
                if (!(pId = TryParsePacketId(pkId)).HasValue)
                {
                    _logger.Log($"Could not find packet id {pkId}", LogLevel.Error);
                    return;
                }

                //Get packet
                var packet = PacketHandler[pId.Value];

                //Check if packet exists
                if (packet == null)
                {
                    _logger.Log($"Could not get packet {pkId}", LogLevel.Error);
                    return;
                }
                _logger.Log($"Found packet {pId}");

                //create writer and reader + set reader position
                PacketReader reader = new PacketReader(data);
                reader.Position = 2;
                PacketWriter writer = new PacketWriter();

                //run packet
                writer = packet.ReadData(reader, writer, this);

                //Check if we should respond
                if (writer != null)
                {
                    _logger.Log($"Responding to packet {pId}");
                    writer.Position = 0;
                    writer.Write((short)packet.PacketId);

                    SendData(writer.ToBytes());

                    writer = packet.OnAfterPacketSent(new PacketWriter(), this);

                    if (writer != null)
                    {
                        _logger.Log($"Sending additional data after packet: {pId}");
                        SendData(writer.ToBytes());
                    }
                }
                    
                _logger.Log($"Finished processing packet {pId}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error occurred while processing a packet: " + ex.ToString(), LogLevel.Error);
            }
        }

        /// <summary>
        /// Encrypts data if encryption is available and then sends the data
        /// </summary>
        public void SendData(byte[] data)
        {
            //If encryption available, encrypt data
            if (Encryption != null && Encryption.Key != null && Encryption.IV != null)
                data = Encryption.Encrypt(data);

            _logger.Log($"Sending {data.Length} bytes");

            Send(data);
        }

        private PacketId? TryParsePacketId(short data)
        {
            if (Enum.IsDefined(typeof(PacketId), data))
                return (PacketId)data;

            return null;
        }

        protected override void OnDisconnected()
        {
            //TODO: notify authenticated players about disconnected player
            Server.OnUserDisconnected(AccountData?.Id ?? -1);
            ClearUserData();
            base.OnDisconnected();
        }

        /// <summary>
        /// Clears the user specific data
        /// </summary>
        private void ClearUserData()
        {
            AccountData = null;
            _isAuthenticated = false;
        }
    }
}
