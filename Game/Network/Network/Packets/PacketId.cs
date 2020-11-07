using Network.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Packets
{
    public enum PacketId : byte
    {
        Ping = 1,

        PublicKey = 2,
        Authenticate = 3,

        
        ErrorPacket = 254,
        InvalidPacket = 255
    }

    [Packet]
    public class ErrorPacket : BasePacket
    {
        public override PacketId PacketId => PacketId.ErrorPacket;
        public string Error;

        public override void Read(BinaryReader reader, Server from, User user)
        {
            throw new Exception(reader.ReadString());
        }

        public override byte[] ToBytes(BinaryWriter writer)
        {
            writer.Write(Error);

            return base.ToBytes(writer);
        }
    }

    [Packet]
    public class AuthenticatePacket : BasePacket
    {
        public override PacketId PacketId => PacketId.Authenticate;

        public User User;

        public override void Read(BinaryReader reader, Server from, User user)
        {
            bool failed = false;

            if (failed)
            {
                ErrorPacket error = new ErrorPacket()
                {
                    Error = "Failed to authenticate"
                };

                from.SendPacketAsync(error, user.Client);
            }
        }

        public override byte[] ToBytes(BinaryWriter writer)
        {
            writer.Write(User.Authenticated);
            return base.ToBytes(writer);
        }
    }

    [Packet]
    public class PublicKeyPacket : BasePacket
    {
        public override PacketId PacketId => PacketId.PublicKey;

        public byte[] Key;

        public override void Read(BinaryReader reader, Server from, User user)
        {
            byte state = reader.ReadByte();

            if (state == 1) //Request the key
            {
                Key = from.GetPublicKey();
                byte[] datagram = ToBytes(new BinaryWriter());

                from.SendAsync(datagram, user.Client);
            }
            else //Received the key
            {
                short length = reader.ReadInt16();
                byte[] key = reader.ReadBytes(length);

                user.EncryptionKey = new EncryptionKey();
                user.EncryptionKey.PublicKey = key;
            }
        }

        public override byte[] ToBytes(BinaryWriter writer)
        {
            writer.Write((byte)PacketId);
            writer.Write(Key.Length);
            writer.Write(Key);

            return base.ToBytes(writer);
        }
    }
}
