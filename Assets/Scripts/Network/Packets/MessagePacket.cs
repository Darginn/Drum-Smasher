using DSServerCommon;
using DSServerCommon.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Network.Packets
{
    public class MessagePacket : BasePacket<User>
    {
        private long _dest;
        private bool _channel;
        private string _message;

        public MessagePacket(ILogger logger) : base(PacketId.Message, logger)
        {

        }

        public MessagePacket(long destination, bool channel, string message, ILogger logger) : base (PacketId.Message, logger)
        {
            _dest = destination;
            _channel = channel;
            _message = message;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            long userId = reader.ReadInt64();
            long dest = reader.ReadInt64();
            bool isChannel = reader.ReadBoolean();
            string msg = reader.ReadString();

            _logger.Log($"Reading chat message from user {userId}");

            from.OnMessageReceived(userId, dest, isChannel, msg);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log("Writing chat message");

            writer.Write(_dest);
            writer.Write(_channel);
            writer.Write(_message);

            return writer;
        }
    }
}
