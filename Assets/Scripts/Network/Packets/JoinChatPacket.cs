using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSServerCommon;
using DSServerCommon.Packets;

namespace DrumSmasher.Network.Packets
{
    public class JoinChatPacket : BasePacket<User>
    {
        private long _channelId;

        public JoinChatPacket(ILogger logger) : base(PacketId.JoinChat, logger)
        {

        }

        public JoinChatPacket(long channelId, ILogger logger) : base(PacketId.JoinChat, logger)
        {

        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            bool status = reader.ReadBoolean();
            long userId = reader.ReadInt64();
            long channelId = reader.ReadInt64();
            _logger.Log($"Reading chat join for user {userId}");

            if (from.AccountData == null || userId == from.AccountData.Id)
                from.OnChatJoined(true);
            else
                from.OnChatJoined(userId);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log("Writing chat join");

            writer.Write(_channelId);
            return writer;
        }
    }
}
