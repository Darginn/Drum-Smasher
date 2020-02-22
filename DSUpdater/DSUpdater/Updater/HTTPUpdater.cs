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
        private System.Net.WebClient _webClient;

        public HTTPUpdater()
        {
            _webClient = new System.Net.WebClient();
        }
        
        public Dictionary<string, string> FileChecksums => throw new NotImplementedException();

        public bool Check(string checksum, string file)
        {
            FileChecksum cs = new FileChecksum(file);
            FileChecksum csM = new FileChecksum("", checksum);

            return csM.Equals(cs);
        }

        public void Dispose()
        {
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }

        public Task<byte[]> DownloadAsync(string host, params string[] path)
        {
            return Task.Run(() =>
            {
                string url = ResolvePath(host, path);
                return _webClient.DownloadData(url);
            });
        }

        public Task<string> DownloadStringAsync(string host, params string[] path)
        {
            return Task.Run(() =>
            {
                string url = ResolvePath(host, path);
                return _webClient.DownloadString(url);
            });
        }

        public Task DownloadFileAsync(string host, string dest, params string[] path)
        {
            return Task.Run(() =>
            {
                string url = ResolvePath(host, path);
                _webClient.DownloadFile(url, dest);
            });
        }

        private string ResolvePath(string host, params string[] path)
        {
            string url = host.TrimEnd('/') + "/" + path;

            for (int i = 1; i < path.Length; i++)
                url += "/" + path[i];

            return url;
        }
    }
}
