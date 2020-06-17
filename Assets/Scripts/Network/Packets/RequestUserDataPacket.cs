using DSServerCommon;
using DSServerCommon.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Network.Packets
{
    public class RequestUserDataPacket : BasePacket<User>
    {
        private long _userId;

        public RequestUserDataPacket(ILogger logger) : base(PacketId.RequestUserData, logger)
        {

        }

        public RequestUserDataPacket(long userId, ILogger logger) : base(PacketId.RequestUserData, logger)
        {
            _userId = userId;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            AccountData data = new AccountData()
            {
                Id = reader.ReadInt64(),
                Name = reader.ReadString(),
                IsAdmin = reader.ReadBoolean()
            };

            from.OnUserData(data);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            writer.Write(_userId);
            return writer;
        }
    }
}
