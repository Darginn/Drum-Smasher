using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Network.Packets
{
    public enum PacketId
    {
        ChatMessage = 1,
        ChatJoin,
        ChatExit,
        RequestCredents
    }
}
