using DSServerCommon;
using DSServerCommon.Encryption;
using DSServerCommon.Packets;
using System.Security.Cryptography;

namespace DrumSmasher.Network.Packets
{
    public class EncryptionPacket : BasePacket<User>
    {
        private byte _state;
        private User _user;

        public EncryptionPacket(ILogger logger) : base(PacketId.Encryption, logger)
        {

        }

        public EncryptionPacket(User user, ILogger logger) : base(PacketId.Encryption, logger)
        {
            _user = user;
        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, User from)
        {
            switch(reader.ReadByte())
            {
                default:
                    throw new System.Exception("Unsupported encryption state");

                case 2:
                    _logger.Log($"Client rsa key: {from.TempRSAKey}");
                    RSAEncryption rsa = new RSAEncryption(from.TempRSAKey, true);

                    byte[] key = reader.ReadBytes(reader.ReadInt32());
                    byte[] iv = reader.ReadBytes(reader.ReadInt32());

                    key = rsa.Decrypt(key);
                    iv = rsa.Decrypt(iv);

                    from.Encryption = new RijndaelEncryption(key, iv);

                    _logger.Log($"Set encryption: key {key.Length}, iv {iv.Length}");
                    break;
            }

            return null;
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log("Writing encryption");

            switch (_state)
            {
                default:
                    throw new System.Exception("Unsupported encryption state");

                case 0:
                    RSAEncryption rsa = new RSAEncryption();
                    writer.Write((byte)1);
                    writer.Write(rsa.PublicXml);
                    _user.TempRSAKey = rsa.PublicPrivateXml;
                    _logger.Log($"Client rsa key: {_user.TempRSAKey}");
                    return writer;
            }
        }
    }
}
