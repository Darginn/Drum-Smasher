using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSUpdater.Updater
{
    public class HTTPDownloader : IDisposable
    {
        private WebClient _web;
        private string _url;
        private Dictionary<string, string> _toDownload;

        private List<string> _failed;
        private int _downloadLeft;

        public IReadOnlyList<string> FailedDownloads => _failed;
        public int ToDownload => _downloadLeft;
        
        public HTTPDownloader(string url, Dictionary<string, string> toDownload)
        {
            _url = url;
            _toDownload = toDownload;

            _web = new WebClient();
            _failed = new List<string>();
        }

        public void Dispose()
        {
            _web?.Dispose();
        }

        /// <summary>
        /// ToDo, implement multithreaded downloads
        /// </summary>
        /// <param name="threads"></param>
        /// <returns></returns>
        public async Task StartDownloadAsync(int threads = 4)
        {
            await DownloadDictAsync(_toDownload);
        }

        private async Task DownloadDictAsync(Dictionary<string, string> downloads)
        {
            int retries = 0;
            while (downloads.Count > 0)
            {
                string path = downloads.Keys.ElementAt(0);
                string urlPath = downloads[path];

                string url = _url + urlPath;

                await DownloadFileAsync(url, path);

                if (retries == 4)
                {
                    downloads.Remove(path);
                    retries = 0;

                    Console.WriteLine($"Could not download file {url} to {path} after 3 retries, skipping...");
                    continue;
                }
                else if (!File.Exists(path))
                {
                    retries++;
                    Console.WriteLine($"Failed to download file {url} to {path}, retrying... ({retries}/3)");

                    continue;
                }

                downloads.Remove(path);
            }
        }

        private async Task DownloadFileAsync(string file, string path)
        {
            await _web.DownloadFileTaskAsync(file, path);
        }

        public void StopDownloadAsync()
        {

        }
    }
}
