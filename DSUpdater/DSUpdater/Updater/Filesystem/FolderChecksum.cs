using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DSUpdater.Updater.Filesystem
{
    public class FolderChecksum : IEquatable<FolderChecksum>
    {
        public IReadOnlyList<FileChecksum> Files => _files;
        public string Folder => _folder;

        private List<FileChecksum> _files;
        private string _folder;

        public FolderChecksum(string folder)
        {
            _files = new List<FileChecksum>();
            _folder = folder;
        }

        public void GenerateChecksums()
        {
            _files.Clear();

            DirectoryInfo dir = new DirectoryInfo(_folder);

            if (!dir.Exists)
            {
                Console.WriteLine($"Directory {dir.FullName} not found");
                return;
            }

            foreach(FileInfo file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
                _files.Add(new FileChecksum(file.Directory.FullName, file.Name));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FolderChecksum);
        }

        public bool Equals(FolderChecksum other)
        {
            return other != null &&
                   EqualityComparer<List<FileChecksum>>.Default.Equals(_files, other._files) &&
                   _folder == other._folder;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_files, _folder);
        }

        public static bool operator ==(FolderChecksum checksum1, FolderChecksum checksum2)
        {
            return EqualityComparer<FolderChecksum>.Default.Equals(checksum1, checksum2);
        }

        public static bool operator !=(FolderChecksum checksum1, FolderChecksum checksum2)
        {
            return !(checksum1 == checksum2);
        }
    }
}
