using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class VersionInfo : IEquatable<VersionInfo>
    {
        public short[] Version { get; set; }

        public VersionInfo(int length = 4)
        {
            Version = new short[4];
        }

        public VersionInfo(short[] version)
        {
            Version = version;
        }

        public VersionInfo()
        {

        }

        public static VersionInfo FromFile(FileInfo file, int length = 4)
        {
            if (file == null || !file.Exists)
                return new VersionInfo(length);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<VersionInfo>(File.ReadAllText(file.FullName));

        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(Version[0].ToString());
            for (int i = 1; i < Version.Length; i++)
                result.Append($".{Version[i]}");

            return result.ToString();
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return Equals(obj as VersionInfo);
        }

        public bool Equals(VersionInfo other)
        {
            if (Version == null ||
                other == null ||
                other.Version == null ||
                other.Version.Length != Version.Length)
                return false;

            for (int i = 0; i < Version.Length; i++)
                if (Version[i] != other.Version[i])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version);
        }

        public static bool operator ==(VersionInfo left, VersionInfo right)
        {
            return EqualityComparer<VersionInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(VersionInfo left, VersionInfo right)
        {
            return !(left == right);
        }

        public static bool operator >(VersionInfo left, VersionInfo right)
        {
            if (left == null || right == null ||
                left.Version == null || right.Version == null ||
                left.Version.Length != right.Version.Length)
                return false;

            for (int i = 0; i < left.Version.Length; i++)
                if (left.Version[i] < right.Version[i])
                    return false;

            if (left.Version[left.Version.Length - 1] == right.Version[left.Version.Length - 1])
                return false;

            return true;
        }

        public static bool operator <(VersionInfo left, VersionInfo right)
        {
            if (left == null || right == null ||
                left.Version == null || right.Version == null ||
                left.Version.Length != right.Version.Length)
                return false;

            for (int i = 0; i < left.Version.Length; i++)
                if (left.Version[i] < right.Version[i])
                    return true;

            return false;
        }
    }
}
