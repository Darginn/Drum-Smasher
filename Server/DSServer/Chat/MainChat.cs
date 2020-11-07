using DSServer.Server.Packets;
using DSServerCommon;
using DSServerCommon.ChatSystem;
using DSServerCommon.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.Chat
{
    public class MainChat : ChatChannel
    {
        private ILogger _logger;

        public MainChat(long id, string name, ILogger logger) : base(id, name)
        {
            _logger = logger;
        }

        public override void OnChatMessageReceived(ChatUser user, ChatMessage message)
        {
            _logger.Log($"Received chat message from user {user.Id}:{user.Name}");
            MessagePacket mp = new MessagePacket(user.Id, message.Destination, message.IsChannel, message.Message, _logger);
            PacketWriter writer = mp.WriteData(new PacketWriter());
            mp.InsertPacketId(ref writer);
            byte[] toSend = writer.ToBytes();

            Action<int> toRun = new Action<int>(i =>
            {
                try
                {
                    if (!Program.Server.Sessions.TryGetValue(_users[i].Id, out DSSession session))
                        return;

                    _logger.Log($"Sending message to {_users[i].Id}");

                    session.SendData(toSend);
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.ToString(), LogLevel.Error);
                }
            });

            lock(_userCollection.SyncRoot)
            {
                for (int i = 0; i < _users.Count; i++)
                {
                    int index = i;
                    Task.Run(() => toRun(index));
                }
            }
        }

        public override void OnUserJoined(ChatUser user, bool success)
        {
            if (!Program.Server.Sessions.TryGetValue(user.Id, out DSSession session))
            {
                this.OnUserLeave(user);
                return;
            }

            JoinChatPacket jcp = new JoinChatPacket(user.Id, Id, success, _logger);
            PacketWriter writer = jcp.WriteData(new PacketWriter());
            jcp.InsertPacketId(ref writer);

            session.SendData(writer.ToBytes());   
        }

        public override void OnUserLeaved(ChatUser user, bool success)
        {
            if (!Program.Server.Sessions.TryGetValue(user.Id, out DSSession session))
                return;

            PartChatPacket pcp = new PartChatPacket(user.Id, Id, _logger);
            PacketWriter writer = pcp.WriteData(new PacketWriter());
            pcp.InsertPacketId(ref writer);

            session.SendData(writer.ToBytes());
        }
    }
}
