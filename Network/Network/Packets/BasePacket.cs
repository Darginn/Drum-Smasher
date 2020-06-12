using Network.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Network.Packets
{
    public abstract class BasePacket
    {
        public abstract PacketId PacketId { get; }

        public BasePacket()
        {

        }

        public virtual void Read(BinaryReader reader, Server from, User user)
        {
        }

        public virtual byte[] ToBytes(BinaryWriter writer)
        {
            return writer.ToBytes();
        }
    }
}
