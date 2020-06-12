using Network.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Packets.PacketList
{
    public class PingPacket : BasePacket
    {
        public override PacketId PacketId => PacketId.Ping;
        public bool Ping;

        public PingPacket() : base()
        {

        }

        public override void Read(BinaryReader reader, Server from, User user)
        {
            string ping = reader.ReadString();

            if (ping.Equals("ping", StringComparison.CurrentCultureIgnoreCase))
                from.SendPacketAsync(this, user.Client);
        }

        public override byte[] ToBytes(BinaryWriter writer)
        {
            writer.Write((byte)PacketId);

            if (Ping)
                writer.Write("Ping");
            else
                writer.Write("Pong");

            return base.ToBytes(writer);
        }
    }
}
