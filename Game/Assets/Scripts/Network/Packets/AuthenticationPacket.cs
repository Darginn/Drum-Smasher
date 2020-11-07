using DSServerCommon;
using DSServerCommon.Packets;
using System;

namespace DrumSmasher.Network.Packets
{
    public class AuthenticationPacket : BasePacket<User>
    {
        private string _account;
        private string _password;

        public AuthenticationPacket(ILogger logger) : base(PacketId.Authentication, logger)
        {

        }

        public AuthenticationPacket(string account, string password, ILogger logger) : base(PacketId.Authentication, logger)
        {
            _account = account;
            _password = password;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            _logger.Log("Reading authentication");
            bool authenticated = reader.ReadBoolean();
            from.OnAuthenticated(authenticated);

            if (!authenticated)
                return null;

            AccountData data = new AccountData()
            {
                Id = reader.ReadInt64(),
                Name = reader.ReadString(),
                IsAdmin = reader.ReadBoolean()
            };

            from.OnAccountInfoReceived(data);

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            if (string.IsNullOrEmpty(_account))
                throw new NullReferenceException(nameof(_account));
            else if (string.IsNullOrEmpty(_password))
                throw new NullReferenceException(nameof(_password));

            _logger.Log("Writing authentication");

            writer.Write(_account);
            writer.Write(_password);

            return writer;
        }
    }
}
