using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Packets.PacketList
{
    public class InvalidPacket : BasePacket
    {
        public override PacketId PacketId => PacketId.InvalidPacket;

        public InvalidPacket() : base()
        {

        }
    }
}
