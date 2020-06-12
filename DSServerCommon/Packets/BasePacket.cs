using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Packets
{
    public class BasePacket<T>
    {
        public PacketId PacketId { get; }

        protected ILogger _logger;

        public BasePacket(PacketId packetId, ILogger logger)
        {
            PacketId = packetId;
            _logger = logger;
        }

        public virtual PacketWriter ReadData(PacketReader reader, PacketWriter writer, T from)
        {
            return null;
        }

        public virtual PacketWriter WriteData(PacketWriter writer)
        {
            return null;
        }

        public virtual PacketWriter OnAfterPacketSent(PacketWriter writer, T from)
        {
            return null;
        }

        public void InsertPacketId(ref PacketWriter writer)
        {
            writer.Position = 0;
            writer.Write((short)PacketId);
            writer.Position = writer.Count - 1;
        }
    }
}
