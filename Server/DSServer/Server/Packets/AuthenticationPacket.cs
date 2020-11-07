using System;
using DSServerCommon;
using DSServerCommon.Packets;
using DSServer.Database;
using DSServer.Database.Models;
using System.Linq;

namespace DSServer.Server.Packets
{
    public class AuthenticationPacket : BasePacket<DSSession>
    {
        private bool _result;
        private Account _acc;

        public AuthenticationPacket(ILogger logger) : base(PacketId.Authentication, logger)
        {

        }

        public AuthenticationPacket(bool result, Account acc, ILogger logger) : base(PacketId.Authentication, logger)
        {
            _result = result;
            _acc = acc;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, DSSession from)
        {
            string accName = reader.ReadString();
            string accPass = reader.ReadString();

            _logger.Log($"Received authentication with account name: {accName}");

            Account acc = Login(accName, accPass);

            if (acc == null)
            {
                _logger.Log($"Account not found | {from.IPEndPoint.ToString()}", LogLevel.Error);
                _result = false;
                return WriteData(writer);
            }

            from.OnAuthenticated(acc);

            _result = true;
            _acc = acc;

            return WriteData(writer);
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            writer.Write(_result);

            if (!_result)
            {
                _logger.Log($"Failed to authenticate");
                return writer;
            }

            _logger.Log($"Authenticated {_acc.Id}:{_acc.AccountName}");

            writer.Write(_acc.Id);
            writer.Write(_acc.AccountName);
            writer.Write(_acc.PermissionLevel == 1 || _acc.PermissionLevel == 2);

            return writer;
        }

        public override PacketWriter OnAfterPacketSent(PacketWriter writer, DSSession from)
        {
            if (!_result)
                return null;

            if (!Program.Server.Sessions.TryAdd(_acc.Id, from))
                _logger.Log("Failed to add client to sessions", LogLevel.Error);

            _logger.Log($"Authenticating and sending AccountData to {_acc.Id}:{_acc.AccountName}");

            from.ChatUser = new DSServerCommon.ChatSystem.ChatUser(_acc.Id, _acc.AccountName, new float[3] { 255, 0, 0 });

            UserDataPacket udp = new UserDataPacket(_logger, new AccountData[] 
            { 
                new AccountData()
                {
                    Id = _acc.Id,
                    Name = _acc.AccountName,
                    IsAdmin = _acc.PermissionLevel == 1 || _acc.PermissionLevel == 2
                }
            });
            writer = udp.WriteData(writer);
            udp.InsertPacketId(ref writer);

            System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var session in from.Server.Sessions.Values.Where(s => s.IsAuthenticated && s.AccountData.Id != from.AccountData.Id).ToList())
                    session.SendData(writer.ToBytes());
            });

            return null;
        }

        private Account Login(string account, string pass)
        {
            using (DSContext c = new DSContext())
            {
                Account acc = c.Account.FirstOrDefault(ac => ac.AccountName.Equals(account, StringComparison.CurrentCultureIgnoreCase));

                if (acc == null)
                    return null;

                string hash = BCrypt.Net.BCrypt.HashPassword(pass, acc.PasswordSalt);

                if (hash.Equals(acc.PasswordHash, StringComparison.CurrentCulture))
                    return acc;

                return null;
            }
        }
    }
}
