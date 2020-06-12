using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSServerCommon;
using DSServerCommon.Packets;

namespace DrumSmasher.Network.Packets
{
    public class PartChatPacket : BasePacket<User>
    {
        private long _channelId;

        public PartChatPacket(ILogger logger) : base(PacketId.PartChat, logger)
        {

        }

        public PartChatPacket(long channelId, ILogger logger) : base(PacketId.PartChat, logger)
        {

        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            long userId = reader.ReadInt64();
            long channelId = reader.ReadInt64();

            if (userId == from.AccountData.Id)
                from.OnChatParted(true);
            else
                from.OnChatParted(userId);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            writer.Write(_channelId);
            return writer;
        }
    }
}
