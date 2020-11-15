using DSServerCommon.Network.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon.Network.Packets
{
    internal class SEncryptPacket : Packet
    {
        int _state;
        byte[] _rjnKey;
        byte[] _rjnIV;

        public SEncryptPacket(ref byte[] data, NetState state) : base(ref data, state, (int)PacketId.SEncrypt)
        {
            _state = ReadByte();

            switch (_state)
            {
                default:
                    _state = -1;
                    break;

                case 1:
                    byte checkByte = ReadByte();

                    if (checkByte != 0xFF)
                    {
                        _state = -1;
                        break;
                    }
                    break;

                case 3:
                    int length = ReadInt();
                    _rjnKey = state.RSA.Decrypt(ReadBytes(length));

                    length = ReadInt();
                    _rjnIV = state.RSA.Decrypt(ReadBytes(length));
                    break;
            }
        }

        public SEncryptPacket(string rsaKey) : base((int)PacketId.CEncrypt)
        {
            Write((byte)2); //state
            Write(rsaKey);
        }

        public SEncryptPacket(bool confirm) : base((int)PacketId.CEncrypt)
        {
            Write((byte)4);
            Write(confirm);
        }

        public override void InvokePacket(NetState state)
        {
            switch (_state)
            {
                default:
                    break;

                case 1:
                    SendRSA(state);
                    break;

                case 3:
                    SendConfirmation(state);
                    break;
            }
        }

        void SendRSA(NetState state)
        {
            state.RSA = new RSAEncryption();

            SEncryptPacket response = new SEncryptPacket(state.RSA.ToXml(false));
            state.Write(response);
        }

        void SendConfirmation(NetState state)
        {
            state.Rijndael = new RijndaelEncryption(_rjnKey, _rjnIV);

            SEncryptPacket response = new SEncryptPacket(true);
            state.Write(response);

            while (state.WriterQueueCount != 0)
                System.Threading.Tasks.Task.Delay(25).ConfigureAwait(false).GetAwaiter().GetResult();

            state.UseEncryption = true;
        }
    }
}
