using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Packets
{
    public class PacketHandler<T>
    {
        public ConcurrentDictionary<PacketId, BasePacket<T>> Packets { get; private set; }

        public PacketHandler(ILogger logger)
        {
            Packets = new ConcurrentDictionary<PacketId, BasePacket<T>>();
            RegisterDefaultPackets(logger);
        }

        public BasePacket<T> this[PacketId pId]
        {
            get
            {
                if (!Packets.TryGetValue(pId, out BasePacket<T> p))
                    return null;

                return p;
            }
        }

        protected virtual void RegisterDefaultPackets(ILogger logger)
        {

        }
    }
}
