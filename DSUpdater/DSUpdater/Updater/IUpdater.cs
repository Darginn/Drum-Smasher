using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DSUpdater.Updater
{
    public interface IUpdater
    {
        char PathSplitter { get; }
        Dictionary<string, string> FileChecksums { get; }

        Task<byte[]> DownloadAsync(string host, params string[] path);

        bool Check(string checksumA, string file);
    }
}
