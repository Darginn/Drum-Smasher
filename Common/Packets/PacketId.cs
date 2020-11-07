using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Packets
{
    public enum PacketId : short
    {
        Encryption = 1,
        Authentication,
        Message,
        JoinChat,
        PartChat,
        UserData,
    }
}
