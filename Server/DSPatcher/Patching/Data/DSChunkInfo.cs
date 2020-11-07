using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class DSChunkInfo : IEquatable<DSChunkInfo>
    {
        public byte[] Checksum { get; set; }
        public long Start { get; set; }
        public int Length { get; set; }

        public DSChunkInfo(byte[] checksum, long start, int length)
        {
            Checksum = checksum;
            Start = start;
            Length = length;
        }

        public DSChunkInfo()
        {
        }

        public static DSChunkInfo FromBytes(byte[] data, long start)
        {
            byte[] checksum = null;

            using (MD5 md = MD5.Create())
                checksum = md.ComputeHash(data);

            return new DSChunkInfo(checksum, start, data.Length);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DSChunkInfo);
        }

        public bool Equals([AllowNull] DSChunkInfo other)
        {
            if (other == null ||
                other.Checksum == null ||
                Checksum == null ||
                Checksum.Length != other.Checksum.Length)
                return false;

            for (int i = 0; i < Checksum.Length; i++)
                if (Checksum[i] != other.Checksum[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Checksum, Start, Length);
        }

        public static bool operator ==(DSChunkInfo left, DSChunkInfo right)
        {
            return EqualityComparer<DSChunkInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(DSChunkInfo left, DSChunkInfo right)
        {
            return !(left == right);
        }
    }
}
