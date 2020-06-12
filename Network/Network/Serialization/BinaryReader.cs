using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Serialization
{
    public class BinaryReader
    {
        public int Position { get; set; }

        private byte[] _data;

        public BinaryReader(byte[] data)
        {
            _data = data;
        }

        public byte ReadByte()
        {
            byte b = _data[Position];
            Position++;

            return b;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(_data, Position, bytes, 0, length);

            Position += length;

            return bytes;
        }

        public Int16 ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }
        public Int32 ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }
        public Int64 ReadInt64()
        {
            return BitConverter.ToInt64(ReadBytes(8), 0);
        }

        public UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(2), 0);
        }
        public UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }
        public UInt64 ReadUInt64()
        {
            return BitConverter.ToUInt64(ReadBytes(8), 0);
        }

        public Single ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }
        public Double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(8), 0);
        }

        public Boolean ReadBoolean()
        {
            return ReadByte() == (byte)1 ? true : false;
        }

        public String ReadString()
        {
            int length = ReadInt32();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }
    }
}
