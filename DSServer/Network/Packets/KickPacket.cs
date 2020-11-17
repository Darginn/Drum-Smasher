using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Network.Packets
{
    public class KickPacket : Packet
    {
        public KickPacket(string reason, bool isBan) : base((int)PacketId.Kick)
        {
            Write(reason);
            Write(isBan);
        }

        public KickPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.Kick)
        {

        }

        public override void InvokePacket(NetState state)
        {

        }
    }
}
