using DSServerCommon.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Network
{
    public static class PacketHandler
    {
        /// <summary>
        /// Register any packets here
        /// <para>Packet id <see cref="int.MaxValue"/> (2147483647) and <see cref="int.MaxValue"/> - 1 (2147483646) are reserved for encryption</para>
        /// </summary>
        public static Dictionary<int, Type> Packets { get; private set; }

        public static void Initialize()
        {
            Packets = new Dictionary<int, Type>()
            {
                { (int)PacketId.CEncrypt, typeof(CEncryptPacket) },
                { (int)PacketId.SEncrypt, typeof(SEncryptPacket) },
            };
        }
    }
}
