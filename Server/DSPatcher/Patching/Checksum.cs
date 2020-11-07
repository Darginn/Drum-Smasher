using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace DSPatcher.Patching
{
    public class Checksum : IDisposable, IEquatable<Checksum>
    {
        public bool IsDisposed { get; private set; }
        public byte[] Value { get; private set; }

        private MD5 _md5;

        public Checksum(MD5 md5 = null)
        {
            if (md5 != null)
                _md5 = md5;
            else
                _md5 = MD5.Create();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            _md5?.Dispose();

            IsDisposed = true;
        }

        public void Generate(byte[] data)
        {
            Value = _md5.ComputeHash(data);
        }

        public static Checksum FromStream(byte[] data, MD5 md5 = null)
        {
            Checksum cs = new Checksum(md5);
            cs.Generate(data);

            return cs;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Checksum);
        }

        public bool Equals([AllowNull] Checksum other)
        {
            if (other == null)
                return false;

            return Equals(other.Value);
        }

        public bool Equals([AllowNull] byte[] other)
        {
            if (other == null ||
                Value == null ||
                Value.Length != other.Length)
                return false;

            for (int i = 0; i < Value.Length; i++)
                if (Value[i] != other[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(Checksum left, Checksum right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Checksum left, Checksum right)
        {
            return !(left == right);
        }
    }
}
