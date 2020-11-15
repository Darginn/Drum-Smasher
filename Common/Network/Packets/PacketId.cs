using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Network.Packets
{
    internal enum PacketId
    {
        //Encryption
        SEncrypt = int.MaxValue - 1,
        CEncrypt = int.MaxValue,
    }
}
