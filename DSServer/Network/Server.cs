using DSServerCommon.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServer.Network
{
    public class Server : Listener
    {
        public Server(string host, int port) : base(host, port)
        {

        }

        public void Start()
        {
            RegisterPackets();
            Start<Client>();
        }

        void RegisterPackets()
        {
            PacketHandler.Initialize();
        }
    }
}
