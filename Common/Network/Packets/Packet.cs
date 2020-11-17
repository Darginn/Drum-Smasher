using DSServerCommon.Network.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSServerCommon.Network.Packets
{
    public class Packet
    {
        public int Id { get; private set; }
        public int Position { get; set; }
        public int Length => _data?.Count ?? 0;

        List<byte> _data;

        public Packet(int id) : this()
        {
            _data = new List<byte>();
            Id = id;
        }

        protected Packet(ref byte[] data, NetState state, int id)
        {
            Id = id;
            _data = data.ToList();
            _data.RemoveRange(0, 4); //remove packet id

            if (state.UseEncryption)
            {
               _data = state.Rijndael.Decrypt(_data.ToArray()).ToList();
            }
        }

        Packet()
        {
        }

        public virtual void InvokePacket(NetState state)
        {

        }

        public List<byte> ToList(IEncryption crypt = null)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Id));

            if (crypt != null)
            {
                byte[] encData = crypt.Encrypt(_data.ToArray());
                bytes.AddRange(encData);
            }
            else 
                bytes.AddRange(_data);

            bytes.AddRange(new byte[]
            {
                0xFF,
                0xFA,
                0xFF,
                0xFA,
                0xFF
            });

            return bytes;
        }

        public byte[] ToArray(IEncryption crypt = null)
        {
            return ToList(crypt).ToArray();
        }

        #region reader
        protected byte ReadByte()
        {
            Position++;
            return _data[Position - 1];
        }

        protected byte[] ReadBytes(int length)
        {
            byte[] result = new byte[length];

            for (int i = Position; i < Position + length; i++)
                result[i - Position] = _data[i];

            Position += length;
            return result;
        }

        protected Guid ReadGuid()
        {
            return new Guid(ReadBytes(16));
        }

        protected short ReadShort()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        protected int ReadInt()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        protected long ReadLong()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        protected string ReadString()
        {
            int length = ReadInt();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        protected bool ReadBool()
        {
            return ReadByte() == 1;
        }

        protected double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        protected float ReadFloat()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }
        #endregion

        #region writer
        protected void Write(byte b)
        {
            _data.Add(b);
        }

        protected void Write(IEnumerable<byte> bytes)
        {
            _data.AddRange(bytes);
        }

        protected void Write(short v)
        {
            Write(BitConverter.GetBytes(v));
        }

        protected void Write(Guid v)
        {
            Write(v.ToByteArray());
        }

        protected void Write(int v)
        {
            Write(BitConverter.GetBytes(v));
        }

        protected void Write(long v)
        {
            Write(BitConverter.GetBytes(v));
        }

        protected void Write(string v)
        {
            byte[] data = Encoding.UTF8.GetBytes(v);

            Write(data.Length);
            Write(data);
        }

        protected void Write(bool v)
        {
            Write(v ? (byte)1 : (byte)0);
        }

        protected void Write(double v)
        {
            Write(BitConverter.GetBytes(v));
        }

        protected void Write(float v)
        {
            Write(BitConverter.GetBytes(v));
        }
        #endregion
    }
}
