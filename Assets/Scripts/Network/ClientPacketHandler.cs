using DrumSmasher.Network.Packets;
using DSServerCommon.Packets;
using DSServerCommon;
using System.Collections.Generic;

namespace DrumSmasher.Network
{
    public class ClientPacketHandler : PacketHandler<User>
    {
        public ClientPacketHandler(ILogger logger) : base(logger)
        {

        }

        protected override void RegisterDefaultPackets(ILogger logger)
        {
            List<BasePacket<User>> packets = new List<BasePacket<User>>()
            {
                new AccountDataPacket(logger),
                new AuthenticationPacket(logger),
                new EncryptionPacket(logger),
                new JoinChatPacket(logger),
                new MessagePacket(logger),
                new PartChatPacket(logger),
                new RequestUserDataPacket(logger)
            };

            for (int i = 0; i < packets.Count; i++)
                Packets.TryAdd(packets[i].PacketId, packets[i]);
        }
    }
}
