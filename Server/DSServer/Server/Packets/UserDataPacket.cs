using DSServer.Database;
using DSServerCommon;
using DSServerCommon.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSServer.Server.Packets
{
    public class UserDataPacket : BasePacket<DSSession>
    {
        private AccountData[] _data;

        public UserDataPacket(ILogger logger) : base(PacketId.UserData, logger)
        {

        }

        public UserDataPacket(ILogger logger, params AccountData[] data) : base(PacketId.UserData, logger)
        {
            _data = data;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, DSSession from)
        {
            long userId = reader.ReadInt64();
            _logger.Log($"Reading user request for user {userId}");

            using (DSContext c = new DSContext())
            {
                var acc = c.Account.FirstOrDefault(ac => ac.Id == userId);

                if (acc == null)
                    _data = new AccountData[0];
                else
                {
                    _data = new AccountData[]
                    {
                        new AccountData()
                        {
                            Id = acc.Id,
                            IsAdmin = acc.PermissionLevel == 1 || acc.PermissionLevel == 2,
                            Name = acc.AccountName
                        }
                    };
                }
            }

            return WriteData(writer);
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log($"Writing {_data.Length} accounts");

            writer.Write((byte)_data.Length);

            for (int i = 0; i < _data.Length; i++)
            {
                writer.Write(_data[i].Id);
                writer.Write(_data[i].Name);
                writer.Write(_data[i].IsAdmin);
            }
            
            return writer;
        }
    }
}
