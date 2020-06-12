using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Serialization
{
    public class BinaryWriter
    {
        public int Position { get; set; }

        private List<byte> _data;

        public BinaryWriter()
        {

        }

        public void Write(byte v)
        {
            _data.Insert(Position, v);
            Position++;
        }

        public void Write(params byte[] v)
        {
            for (int i = 0; i < v.Length; i++)
                _data.Insert(Position + i, v[i]);

            Position += v.Length;
        }

        public void Write(Int16 v)
        {
            Write(BitConverter.GetBytes(v));
        }
        public void Write(Int32 v)
        {
            Write(BitConverter.GetBytes(v));
        }
        public void Write(Int64 v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(UInt16 v)
        {
            Write(BitConverter.GetBytes(v));
        }
        public void Write(UInt32 v)
        {
            Write(BitConverter.GetBytes(v));
        }
        public void Write(UInt64 v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(Single v)
        {
            Write(BitConverter.GetBytes(v));
        }
        public void Write(Double v)
        {
            Write(BitConverter.GetBytes(v));
        }

        public void Write(Boolean v)
        {
            Write(v ? (byte)1 : (byte)0);
        }

        public void Write(String v)
        {
            Write(v.Length);
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            Write(bytes);
        }

        public byte[] ToBytes()
        {
            return _data.ToArray();
        }
    }
}
