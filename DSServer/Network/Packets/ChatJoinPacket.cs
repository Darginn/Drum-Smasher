using DSServer.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;

namespace DSServer.Network.Packets
{
    public class ChatJoinPacket : Packet
    {
        Guid _chat;

        public ChatJoinPacket(Guid user, Guid chat) : base((int)PacketId.ChatJoin)
        {
            Write(user);
            Write(chat);
        }

        public ChatJoinPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.ChatJoin)
        {
            _chat = ReadGuid();
        }

        public override void InvokePacket(NetState state)
        {
            if (IdentityManager.TryGetChatRoom(_chat, out ChatRoom room))
            {
                room.OnChatJoin((state as Client).ChatUser);
            }
        }
    }
}
