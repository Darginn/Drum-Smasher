using System;
using System.Collections.Generic;
using System.Text;

namespace DSServerCommon
{
    public class StateObject
    {
        public byte[] Bytes => _bytes;
        private byte[] _bytes;

        public StateObject(int length)
        {
            _bytes = new byte[length];
        }

        public void Resize(int length)
        {
            if (length == _bytes.Length || length < 0)
                return;

            Array.Resize(ref _bytes, length);
        }
    }
}
