using DSServer.Network.Packets;
using DSServerCommon.Network;
using System;
using System.Collections.Generic;
using System.Linq;
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

            PacketHandler.Packets.Add((int)PacketId.ChatMessage, typeof(ChatMessagePacket));
            PacketHandler.Packets.Add((int)PacketId.ChatJoin, typeof(ChatJoinPacket));
            PacketHandler.Packets.Add((int)PacketId.ChatExit, typeof(ChatExitPacket));
            PacketHandler.Packets.Add((int)PacketId.RequestCredents, typeof(RequestCredentsPacket));
        }
    }
}
