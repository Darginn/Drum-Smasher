using System;
using System.Net;

namespace Network.Internal
{
    internal class StateObject
    {
        public byte[] Buffer => _buffer;
        public IPEndPoint From => _from;

        private byte[] _buffer;
        private IPEndPoint _from;

        public StateObject(byte[] buffer, IPEndPoint from)
        {
            _buffer = buffer;
            _from = from;
        }

        //public StateObject(int length)
        //{
        //    _buffer = new byte[length];
        //}

        //public void Resize(int newLength)
        //{
        //    if (newLength == _buffer.Length)
        //        return;

        //    Array.Resize(ref _buffer, newLength);
        //}
    }
}
