using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DSUpdater.Updater.Filesystem
{
    public class FileChecksum : IEquatable<FileChecksum>
    {
        public string Checksum;
        public string File;
        public string Folder;
        
        /// <summary>
        /// Generates the checksum from the file
        /// </summary>
        /// <param name="file"></param>
        public FileChecksum(string file, string folder)
        {
            File = file;
            Folder = folder;

            GenerateChecksum();
        }
        
        public void GenerateChecksum()
        {
            FileInfo fi = new FileInfo(Path.Combine(Folder, File));

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

                checksum = md.ComputeHash(buffer, 0, buffer.Length);
            }

            if (checksum == null)
                throw new Exception("Checksum is null for " + fi.FullName);

            Checksum = "";

            foreach (byte b in checksum)
                Checksum += b;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as FileChecksum);
        }

        public bool Equals(FileChecksum other)
        {
            return other != null &&
                   Checksum == other.Checksum;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Checksum);
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
