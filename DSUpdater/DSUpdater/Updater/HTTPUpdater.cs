using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DSUpdater.Updater
{
    class HTTPUpdater : IUpdater, IDisposable
    {
        public char PathSplitter => _pathSplitter;
        public Dictionary<string, string> FileChecksums => _fileChecksums;
        public bool Disposed => _webClient == null;

        private Dictionary<string, string> _fileChecksums;
        private char _pathSplitter;

        private System.Net.WebClient _webClient;

        public HTTPUpdater()
        {
            _webClient = new System.Net.WebClient();
        }

        public bool Check(string checksum, string file)
        {
            FileChecksum cs = new FileChecksum(file);
            FileChecksum csM = new FileChecksum("", checksum);

            return csM.Equals(cs);
        }

        public async Task<byte[]> DownloadAsync(string host, params string[] path)
        {
            string uri = host;

            foreach (string str in path)
                uri += "/" + str;

            byte[] data = await Task.Run(() => _webClient.DownloadData(uri));

            return data;
        }

        public async Task<string> DownloadStringAsync(string host, params string[] path)
        {
            string uri = host;

            foreach (string str in path)
                uri += "/" + str;

            string data = await Task.Run(() => _webClient.DownloadString(uri));

            return data;
        }

        public void Dispose()
        {
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }
    }
}
