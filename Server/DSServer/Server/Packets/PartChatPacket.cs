using DSServerCommon;
using DSServerCommon.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Server.Packets
{
    public class PartChatPacket : BasePacket<DSSession>
    {
        private long _userId;
        private long _channelId;

        public PartChatPacket(ILogger logger) : base(PacketId.PartChat, logger)
        {

        }

        public PartChatPacket(long userId, long channelId, ILogger logger) : base(PacketId.PartChat, logger)
        {
            _userId = userId;
            _channelId = channelId;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, DSSession from)
        {
            _logger.Log($"Trying to part chat from {from.AccountData?.Id ?? 0}:{from.AccountData?.Name ?? "null"}");

            _channelId = reader.ReadInt64();
            from.TryPartChat(_channelId);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log($"Trying to part chat from {_userId}");

            writer.Write(_userId);
            writer.Write(_channelId);

            return writer;
        }
    }
}
