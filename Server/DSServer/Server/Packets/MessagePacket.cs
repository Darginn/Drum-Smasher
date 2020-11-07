using System;
using System.Collections.Generic;
using System.Text;
using DSServerCommon;
using DSServerCommon.ChatSystem;
using DSServerCommon.Packets;

namespace DSServer.Server.Packets
{
    public class MessagePacket : BasePacket<DSSession>
    {
        private long _userId;
        private long _dest;
        private bool _isChannel;
        private string _message;

        public MessagePacket(ILogger logger) : base(PacketId.Message, logger)
        {

        }

        public MessagePacket(long userId, long dest, bool isChannel, string message, ILogger logger) : base(PacketId.Message, logger)
        {
            _userId = userId;
            _dest = dest;
            _isChannel = isChannel;
            _message = message;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, DSSession from)
        {
            _logger.Log($"Reading message from {from.AccountData.Id}:{from.AccountData.Name}");

            long dest = reader.ReadInt64();
            bool isChannel = reader.ReadBoolean();
            string msg = reader.ReadString();

            ChatMessage cmsg = new ChatMessage(from.AccountData.Id, dest, isChannel, msg);
            from.TrySendMessage(cmsg);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log($"Sending message as {_userId}");

            writer.Write(_userId);
            writer.Write(_dest);
            writer.Write(_isChannel);
            writer.Write(_message);

            return writer;
        }
    }
}
