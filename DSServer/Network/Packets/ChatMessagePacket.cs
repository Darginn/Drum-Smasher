using DSServer.ChatSystem;
using DSServerCommon.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;

namespace DSServer.Network.Packets
{
    public class ChatMessagePacket : Packet
    {
        Guid _destId;
        string _msg;

        public ChatMessagePacket(Guid from, Guid dest, string msg) : base((int)PacketId.ChatMessage)
        {
            Write(from);
            Write(dest);
            Write(msg);
        }

        public ChatMessagePacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.ChatMessage)
        {
            _destId = ReadGuid();
            _msg = ReadString();
        }

        public override void InvokePacket(NetState state)
        {
            if ((state as Client).ChatUser.IsSilenced)
                return;

            if (IdentityManager.TryGetIdentity(_destId, out ChatIdentity identity))
                identity.SendMessage((state as Client).ChatUser, _msg);
        }
    }
}
