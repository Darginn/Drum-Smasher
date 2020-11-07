using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Network.Packets
{
    public static class PacketHandler
    {
        private static ConcurrentDictionary<PacketId, Type> _packets;

        internal static void Initialize()
        {
            _packets = new ConcurrentDictionary<PacketId, Type>();

            RegisterPackets();
        }

        private static void RegisterPackets()
        {
            Assembly currentAssembly = Assembly.GetCallingAssembly();
            Type[] types = currentAssembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                PacketAttribute pAttrib = types[i].GetCustomAttribute<PacketAttribute>();

                if (pAttrib == null)
                    continue;

                BasePacket bp = Activator.CreateInstance(types[i]) as BasePacket;

                _packets.TryAdd(bp.PacketId, types[i]);
            }
        }

        public static BasePacket GetPacket(PacketId packetId)
        {
            if (!_packets.TryGetValue(packetId, out Type pType))
                return null;

            return Activator.CreateInstance(pType) as BasePacket;
        }

    }
}
