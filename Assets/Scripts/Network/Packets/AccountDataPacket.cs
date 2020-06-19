using DSServerCommon;
using DSServerCommon.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Network.Packets
{
    public class AccountDataPacket : BasePacket<User>
    {
        public AccountDataPacket(ILogger logger) : base(PacketId.AccountData, logger)
        {

        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            AccountData accData = new AccountData()
            {
                Id = reader.ReadInt64(),
                Name = reader.ReadString(),
                IsAdmin = reader.ReadBoolean()
            };

            _logger.Log($"Reading account data for {accData.Id}");

            from.OnAccountInfoReceived(accData);

            return null;
        }
    }
}
