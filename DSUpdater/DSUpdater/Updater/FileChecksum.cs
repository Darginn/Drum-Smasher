using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DSUpdater.Updater
{
    public class FileChecksum : IEquatable<FileChecksum>
    {
        public string Checksum => _checksum;
        public string File => _file;

        private string _checksum;
        private string _file;
        
        /// <summary>
        /// Generates the checksum from the file
        /// </summary>
        /// <param name="file"></param>
        public FileChecksum(string file)
        {
            GenerateChecksum(file);
        }

        public FileChecksum(string file, string checksum)
        {
            _file = file ?? "";
            _checksum = checksum ?? "";
        }

        public void GenerateChecksum(string file)
        {
            FileInfo fi = new FileInfo(file);

            if (!fi.Exists)
                throw new Exception("Could not find file at GenerateChecksum");

            MD5 md = MD5.Create();

            byte[] checksum = null;
            byte[] buffer;
            
            using (FileStream fstream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Write))
            {
                buffer = new byte[fstream.Length];
                int read = fstream.Read(buffer, 0, buffer.Length);

                if (read != buffer.Length)
                    Array.Resize(ref buffer, read);

                if (!md.TryComputeHash(buffer, checksum, out int bytesWritten))
                    throw new Exception("Could not compute hash of " + fi.FullName);
            }

            if (checksum == null)
                throw new Exception("Checksum is null for " + fi.FullName);

            _checksum = "";

            foreach (byte b in checksum)
                _checksum += b;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileChecksum);
        }

        public bool Equals(FileChecksum other)
        {
            return other != null &&
                   _checksum == other._checksum;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_checksum);
        }

        public static bool operator ==(FileChecksum checksum1, FileChecksum checksum2)
        {
            return EqualityComparer<FileChecksum>.Default.Equals(checksum1, checksum2);
        }

        public static bool operator !=(FileChecksum checksum1, FileChecksum checksum2)
        {
            return !(checksum1 == checksum2);
        }
    }
}
