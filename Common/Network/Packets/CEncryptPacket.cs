using DSServerCommon.Network.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Network.Packets
{
    internal class CEncryptPacket : Packet
    {
        int _state;
        bool _confirmed;

        public CEncryptPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.CEncrypt)
        {
            _state = ReadByte();

            switch (_state)
            {
                default:
                    _state = -1;
                    break;

                case 2:
                    string rsa = ReadString();
                    state.RSA = new RSAEncryption(rsa);
                    break;

                case 4:
                    _confirmed = ReadBool();
                    break;
            }
        }

        public CEncryptPacket() : base((int)PacketId.SEncrypt)
        {
            Write((byte)1);
            Write((byte)0xFF);
        }

        public CEncryptPacket(byte[] rjnKey, byte[] rjnIv, NetState state) : base((int)PacketId.SEncrypt)
        {
            Write((byte)3);

            byte[] encKey = state.RSA.Encrypt(rjnKey);
            byte[] encIv = state.RSA.Encrypt(rjnIv);

            Write(encKey.Length);
            Write(encKey);

            Write(encIv.Length);
            Write(encIv);
        }

        public override void InvokePacket(NetState state)
        {
            switch (_state)
            {
                default:
                    break;

                case 2:
                    state.Rijndael = new RijndaelEncryption(true);

                    CEncryptPacket encrypt = new CEncryptPacket(state.Rijndael.Key, state.Rijndael.IV, state);
                    state.Write(encrypt);
                    break;

                case 4:
                    if (_confirmed)
                        state.UseEncryption = true;
                    break;
            }
        }
    }
}
