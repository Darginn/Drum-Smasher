using DSServer.ChatSystem;
using DSServerCommon.Network;
using DSServerCommon.Network.Packets;
using System;

namespace DSServer.Network.Packets
{
    public class ChatExitPacket : Packet
    {
        Guid _chat;

        public ChatExitPacket(Guid user, Guid chat) : base((int)PacketId.ChatExit)
        {
            Write(user);
            Write(chat);
        }

        public ChatExitPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.ChatExit)
        {
            _chat = ReadGuid();
        }

        public override void InvokePacket(NetState state)
        {
            if (IdentityManager.TryGetChatRoom(_chat, out ChatRoom room))
            {
                room.OnChatExit((state as Client).ChatUser);
            }
        }
    }
}
