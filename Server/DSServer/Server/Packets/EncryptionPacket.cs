using DSServerCommon;
using DSServerCommon.Encryption;
using DSServerCommon.Packets;
using System.Security.Cryptography;

namespace DSServer.Server.Packets
{
    public class EncryptionPacket : BasePacket<DSSession>
    {
        private byte _state;

        private byte[] _key;
        private byte[] _iv;
        private RijndaelEncryption _encryption;

        public EncryptionPacket(ILogger logger) : base(PacketId.Encryption, logger)
        {

        }

        public override PacketWriter ReadData(PacketReader reader, PacketWriter writer, DSSession from)
        {
            _logger.Log("Received encryption trade part");

            switch (reader.ReadByte())
            {
                default:
                    _logger.Log("Failed to read encryption trade part");
                    return null;

                case 1:
                    RSAEncryption rsa = new RSAEncryption(reader.ReadString(), false);

                    _encryption = new RijndaelEncryption();
                    _key = rsa.Encrypt(_encryption.Key);
                    _iv = rsa.Encrypt(_encryption.IV);
                    _state = 1;

                    return WriteData(writer);

            }
        }

        public override PacketWriter WriteData(PacketWriter writer)
        {
            _logger.Log("Writing encryption trade part");

            switch (_state)
            {
                default:
                    _logger.Log("Failed to write encryption trade part");
                    return null;

                case 1:
                    writer.Write((byte)2);
                    writer.Write(_key.Length);
                    writer.Write(_key);
                    writer.Write(_iv.Length);
                    writer.Write(_iv);
                    return writer;
            }
        }

        public override PacketWriter OnAfterPacketSent(PacketWriter writer, DSSession from)
        {
            from.Encryption = _encryption;

            return null;
        }
    }
}
