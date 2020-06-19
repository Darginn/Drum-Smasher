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
            int count = reader.ReadByte();
            Logger.Log($"Received {count} accounts");

            AccountData data;
            for (int i = 0; i < count; i++)
            {
                data = new AccountData()
                {
                    Id = reader.ReadInt64(),
                    Name = reader.ReadString(),
                    IsAdmin = reader.ReadBoolean()
                };

                from.OnUserData(data);
            }

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log($"Writing request for user data for user {_userId}");

            writer.Write(_userId);
            return writer;
        }
    }
}
