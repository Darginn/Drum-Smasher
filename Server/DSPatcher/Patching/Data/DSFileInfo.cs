using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class DSFileInfo : IEquatable<DSFileInfo>
    {
        public string RelativePath { get; set; }
        public DSChunkInfo[] ChunkInfos { get; set; }
        public long FileLength { get; set; }

        public DSFileInfo(string relativePath, DSChunkInfo[] chunkInfos, long fileLength)
        {
            RelativePath = relativePath;
            ChunkInfos = chunkInfos;
            FileLength = fileLength;
        }

        public DSFileInfo()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DSFileInfo);
        }

        public bool Equals([AllowNull] DSFileInfo other)
        {
            return other != null &&
                   RelativePath == other.RelativePath &&
                   EqualityComparer<DSChunkInfo[]>.Default.Equals(ChunkInfos, other.ChunkInfos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RelativePath, ChunkInfos);
        }

        public static bool operator ==(DSFileInfo left, DSFileInfo right)
        {
            return EqualityComparer<DSFileInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(DSFileInfo left, DSFileInfo right)
        {
            return !(left == right);
        }
    }
}
