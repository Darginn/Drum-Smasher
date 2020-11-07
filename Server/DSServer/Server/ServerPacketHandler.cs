using DSServer.Server.Packets;
using DSServerCommon;
using DSServerCommon.Packets;
using System.Collections.Generic;

namespace DSServer.Server
{
    public class ServerPacketHandler : PacketHandler<DSSession>
    {
        public ServerPacketHandler(ILogger logger) : base(logger)
        {
            
        }

        protected override void RegisterDefaultPackets(ILogger logger)
        {
            List<BasePacket<DSSession>> packets = new List<BasePacket<DSSession>>()
            {
                new AuthenticationPacket(logger),
                new EncryptionPacket(logger),
                new JoinChatPacket(logger),
                new MessagePacket(logger),
                new PartChatPacket(logger),
                new UserDataPacket(logger)
            };

            for (int i = 0; i < packets.Count; i++)
                Packets.TryAdd(packets[i].PacketId, packets[i]);
        }
    }
}
