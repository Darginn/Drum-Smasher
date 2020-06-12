using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSServerCommon;
using DSServerCommon.Packets;

namespace DrumSmasher.Network.Packets
{
    public class UsernameRequestPacket : BasePacket<User>
    {
        private long _userId;

        public UsernameRequestPacket(ILogger logger) : base(PacketId.UsernameRequest, logger)
        {

        }

        public UsernameRequestPacket(long userId, ILogger logger) : base(PacketId.UsernameRequest, logger)
        {
            _userId = userId;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            long userId = reader.ReadInt64();
			
			if (userId < 0)
				return null;
			
            string username = reader.ReadString();

            from.Usernames.AddOrUpdate(userId, f => username, (f, s) => username);
            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            writer.Write(_userId);
            return writer;
        }
    }
}
