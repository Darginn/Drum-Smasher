using DSServerCommon.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Network.Packets
{
    public class ChatMessagePacket : Packet
    {
        Guid _destId;
        string _msg;

        public ChatMessagePacket(Guid from, Guid dest, string msg) : base((int)PacketId.ChatMessage)
        {
            Write(from.ToByteArray());
            Write(dest.ToByteArray());
            Write(msg);
        }

        public ChatMessagePacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.ChatMessage)
        {
            _destId = new Guid(ReadBytes(16));
            _msg = ReadString();
        }

        public override void InvokePacket(NetState state)
        {
            if (ChatIdentity.TryGetIdentity(_destId, out ChatIdentity identity))
                identity.SendMessage((state as Client).ChatUser, _msg);
        }
    }
}
