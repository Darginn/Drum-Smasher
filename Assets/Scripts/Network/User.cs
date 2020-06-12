using DSServerCommon.Encryption;
using DSServerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSServerCommon.Packets;
using DrumSmasher.Network.Packets;
using System.Collections;
using UnityEngine;
using ILogger = DSServerCommon.ILogger;
using Assets.Scripts.Chat;
using System.Collections.Concurrent;

namespace DrumSmasher.Network
{
    public class User : TCPClient
    {
        /// <summary>
        /// Encryption key for data transmission
        /// </summary>
        public RijndaelEncryption Encryption { get; set; }
        /// <summary>
        /// TempRSAKey used for initial key transmission
        /// </summary>
        public string TempRSAKey { get; set; }
        /// <summary>
        /// Packethandler
        /// </summary>
        public PacketHandler<User> PacketHandler { get; private set; }

        /// <summary>
        /// Acc data
        /// </summary>
        public AccountData AccountData { get; private set; }
        /// <summary>
        /// Is user authenticated
        /// </summary>
        public bool IsAuthenticated => _isAuthenticated && AccountData != null;

        public ConcurrentDictionary<long, string> Usernames => _usernames;

        private ConcurrentDictionary<long, string> _usernames;
        private bool _isAuthenticated;
        private ILogger _logger;
        private string _user;
        private string _pass;

        private int _currentChannel;

        public User(IPAddress ip, int port, string user, string pass, ILogger logger) : base(ip, port)
        {
            _user = user;
            _pass = pass;
            _logger = logger;
            _currentChannel = -1;
            _usernames = new ConcurrentDictionary<long, string>();
            PacketHandler = new ClientPacketHandler(logger);
            UIChat.Chat.OnNewUserMessage += OnUserChatMessage;
        }

        private void OnUserChatMessage(object sender, string message)
        {
            if (message[0].Equals('/'))
            {
                OnUserChatCommand(message);
                return;
            }

            if (_currentChannel < 0)
            {
                _logger.Log("Could not send message, did not join any channel yet");
                return;
            }

            SendMessage(_currentChannel, true, message);
        }

        private void OnUserChatCommand(string message)
        {
            string[] split = message.Split(' ');

            switch(split[0].ToLower())
            {
                case "/sysmsg":
                    {
                        string msg = message.Remove(0, 8);
                        UIChat.Chat.SysMsg(msg);
                    }
                    break;

                case "/join":
                    {
                        long chId = long.Parse(split[1]);
                        UIChat.Chat.SysMsg($"Trying to join channel: {chId}");
                        JoinChatPacket jcp = new JoinChatPacket(chId, _logger);
                        PacketWriter writer = jcp.WriteData(new PacketWriter());
                        jcp.InsertPacketId(ref writer);

                        _currentChannel = 1;
                        SendData(writer.ToBytes());
                    }
                    break;

                case "/part":
                    {
                        long chId = long.Parse(split[1]);
                        UIChat.Chat.SysMsg($"Trying to part channel: {chId}");
                        PartChatPacket pcp = new PartChatPacket(chId, _logger);
                        PacketWriter writer = pcp.WriteData(new PacketWriter());
                        pcp.InsertPacketId(ref writer);

                        _currentChannel = -1;
                        SendData(writer.ToBytes());
                    }
                    break;
            }
        }

        /// <summary>
        /// Authenticates async
        /// </summary>
        public IEnumerator AuthenticateCoroutine()
        {
            _logger.Log("Starting cryptographic trade");
            UIChat.Chat.SysMsg("Starting cryptographic trade...");

            EncryptionPacket enc = new EncryptionPacket(this, _logger);
			PacketWriter writer = enc.WriteData(new PacketWriter());
			enc.InsertPacketId(ref writer);

            SendData(writer.ToBytes());

            while (Encryption == null ||
                   Encryption.Key == null ||
                   Encryption.IV == null)
                yield return new WaitForEndOfFrame();

            _logger.Log("Finished cryptographic trade");
            UIChat.Chat.SysMsg("Finished cryptographic trade");

            _logger.Log("Authenticating");
            UIChat.Chat.SysMsg("Authenticating...");

            AuthenticationPacket auth = new AuthenticationPacket(_user, _pass, _logger);
            writer = auth.WriteData(new PacketWriter());
            auth.InsertPacketId(ref writer);

            SendData(writer.ToBytes());

            while (!IsAuthenticated)
                yield return new WaitForEndOfFrame();

            _logger.Log("Authenticated");
            UIChat.Chat.SysMsg("Authenticated");

            while (AccountData == null && AccountData.Name == null)
                yield return new WaitForEndOfFrame();

            OnUserChatCommand("/join 1");
        }

        /// <summary>
        /// Sends data encrypted
        /// </summary>
        public void SendEncrypted(byte[] data)
        {
            Send(Encryption.Encrypt(data));
        }

        protected override void OnDataReceived(byte[] data)
        {
            HandleData(ref data);
        }

        private void HandleData(ref byte[] data)
        {
            try
            {
                //If encryption available, decrypt data
                if (Encryption != null && Encryption.Key != null && Encryption.IV != null)
                {
                    _logger.Log("Decrypting received data");
                    data = Encryption.Decrypt(data);
                }
                //Get packet id
                short pkId = BitConverter.ToInt16(data, 0);
                _logger.Log($"Received packet id {pkId}");
                PacketId? pId;

                //Check if packet id is valid
                if (!(pId = TryParsePacketId(pkId)).HasValue)
                {
                    _logger.Log($"Could not find packet id {pkId}");
                    return;
                }
                _logger.Log($"Found packet id {pkId}");

                //Get packet
                var packet = PacketHandler[pId.Value];

                //Check if packet exists
                if (packet == null)
                {
                    _logger.Log($"Could not get packet {pkId}");
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
                        _logger.Log($"Sending packet {pId} OnAfterPaketSent");
                        SendData(writer.ToBytes());
                    }

                    _logger.Log($"Finished processing packet {pId}");
                }
                else
                    _logger.Log($"Finished processing packet {pId}, nothing to respond");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error occurred while processing a packet: " + ex.ToString());
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

            Send(data);
        }

        private PacketId? TryParsePacketId(short data)
        {
            if (Enum.IsDefined(typeof(PacketId), data))
                return (PacketId)data;

            return null;
        }

        /// <summary>
        /// On auth result
        /// </summary>
        public bool OnAuthenticated(bool status)
        {
            if (!status)
            {
                _logger.Log("Failed to authenticated");
                return false;
            }

            _logger.Log("Authenticated");
            _isAuthenticated = true;
            return true;
        }

        /// <summary>
        /// On acc info received
        /// </summary>
        public void OnAccountInfoReceived(AccountData data)
        {
            AccountData = data;
            _usernames.TryAdd(data.Id, data.Name);
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public override void Disconnect()
        {
            ClearUserData();

            base.Disconnect();
        }

        /// <summary>
        /// On msg received
        /// </summary>
        public void OnMessageReceived(long userId, long dest, bool channel, string message)
        {
            //ToDo: implement multiple channels
            UIChat.Chat.AddLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} {_usernames[userId]}@{dest}: {message}");
        }

        public void OnChatJoined(long userId)
        {
            UIChat.Chat.SysMsg($"User {_usernames[userId]} joined the chat");
        }

        public void OnChatJoined(bool confirmation)
        {
            UIChat.Chat.SysMsg($"Joined chat: {confirmation}");
        }

        public void OnChatParted(long userId)
        {
            UIChat.Chat.SysMsg($"User {_usernames[userId]} parted the chat");
        }

        public void OnChatParted(bool confirmation)
        {
            UIChat.Chat.SysMsg($"Parted chat");
        }

        public void OnChatParted()
        {
            UIChat.Chat.SysMsg("Parted chat");
        }

        public void SendUsernameRequest(long userId)
        {

        }

        /// <summary>
        /// Sends a message
        /// </summary>
        public void SendMessage(long dest, bool channel, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.Log("Could not send message, message is null or empty");
                return;
            }

            MessagePacket msg = new MessagePacket(dest, channel, message, _logger);
            PacketWriter writer = msg.WriteData(new PacketWriter());
            msg.InsertPacketId(ref writer);

            SendEncrypted(writer.ToBytes());
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
