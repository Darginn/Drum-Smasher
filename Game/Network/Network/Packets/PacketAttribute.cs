using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Packets
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PacketAttribute : Attribute
    {
        public PacketAttribute() : base()
        {

        }
    }
}
