using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DSUpdater.Updater
{
    public class HTTPDownloader : IDisposable
    {
        private WebClient _web;
        private string _url;
        private Dictionary<string, string> _toDownload;

        private List<string> _failed;

        public IReadOnlyList<string> FailedDownloads => _failed;
        public int ToDownload => _toDownload.Count;
        public Dictionary<string, string> DownloadDict
        {
            get
            {
                return _toDownload;
            }
            set
            {
                _toDownload = value;
            }
        }
        
        public HTTPDownloader(string url, Dictionary<string, string> toDownload)
        {
            _url = url;
            _toDownload = toDownload;

            _web = new WebClient();
            _failed = new List<string>();
            _threads = new List<(int, Thread, CancellationTokenSource)>();
        }

        public HTTPDownloader(string url)
        {
            _url = url;
            _web = new WebClient();
            _failed = new List<string>();
            _threads = new List<(int, Thread, CancellationTokenSource)>();
        }



        public void Dispose()
        {
            _web?.Dispose();

            lock(_threadsLock)
            {
                foreach (var obj in _threads)
                {
                    if (obj.Item2 == null || obj.Item2.ThreadState == ThreadState.Aborted ||
                        obj.Item2.ThreadState == ThreadState.AbortRequested ||
                        obj.Item2.ThreadState == ThreadState.Stopped ||
                        obj.Item2.ThreadState == ThreadState.StopRequested ||
                        obj.Item2.ThreadState == ThreadState.Suspended ||
                        obj.Item2.ThreadState == ThreadState.SuspendRequested)
                        continue;

                    obj.Item3.Cancel();
                }
            }
        }

        private List<(int, Thread, CancellationTokenSource)> _threads;
        private object _threadsLock = new object();

        /// <summary>
        /// ToDo, implement multithreaded downloads
        /// </summary>
        /// <param name="threads"></param>
        /// <returns></returns>
        public void StartDownloadThreaded(string folder, int threads = 4)
        {
            double threadDiv = _toDownload.Count / (double)threads;
            int perThread = (int)threadDiv;
            int lastThread = perThread;

            if (perThread < threadDiv)
                lastThread++;

            List<Dictionary<string, string>> downloadDictList = new List<Dictionary<string, string>>();

            int curAmount = perThread;
            int index = 0;
            int start = 0;
            for (int i = 0; i < threads; i++)
            {
                start = index;
                if (i == threads - 1)
                    curAmount = lastThread;

                Dictionary<string, string> downloadDict = new Dictionary<string, string>();
                while(index < start + curAmount)
                {
                    string key = _toDownload.Keys.ElementAt(index);
                    string val = _toDownload[key];

                    downloadDict.Add(key, val);
                    index++;
                }

                downloadDictList.Add(downloadDict);
            }

            for (int i = 0; i < downloadDictList.Count; i++)
            {
                var dict = downloadDictList[i];

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                Thread dlThread = new Thread(new ParameterizedThreadStart(DownloadDictThread))
                {
                    Name = "DSUpdater." + i,
                    IsBackground = false,
                    Priority = ThreadPriority.AboveNormal
                };

                _threads.Add((i, dlThread, source));

                dlThread.Start((i, dict, folder));
                Console.WriteLine("Started thread " + i);
            }
        }

        private void DownloadDictThread(object data)
        {
            (int, Dictionary<string, string>, string) dataObj = ((int, Dictionary<string, string>, string))data;
            Dictionary<string, string> dataDict = dataObj.Item2;
            int thread = dataObj.Item1;

            DownloadDictAsync(dataDict, dataObj.Item3).Wait();
            Console.WriteLine("Thread ended " + thread);
            
            lock (_threadsLock)
            {
                for (int i = 0; i < _threads.Count; i++)
                {
                    if (_threads[i].Item1 != thread)
                        continue;

                    _threads.RemoveAt(i);
                    break;
                }
            }
        }

        public async Task DownloadDictAsync(Dictionary<string, string> downloads, string folder)
        {
            using (WebClient wc = new WebClient())
            {
                int retries = 0;
                while (downloads.Count > 0)
                {
                    string path = downloads.Keys.ElementAt(0);
                    string urlPath = downloads[path];

                    string url = urlPath;
                    string filePath = Path.Combine(folder, path.TrimStart(@"\"[0]));

                    FileInfo fi = new FileInfo(filePath);

                    if (!fi.Directory.Exists)
                        fi.Directory.Create();

                    await DownloadFileAsync(wc, url, filePath);

                    if (retries == 4)
                    {
                        if (File.Exists(path))
                        {
                            downloads.Remove(path);
                            _toDownload.Remove(path);
                            continue;
                        }
                        downloads.Remove(path);
                        _toDownload.Remove(path);
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
                    _toDownload.Remove(path);
                }
            }
        }

        public async Task DownloadFileAsync(WebClient client, string file, string path)
        {
            await client.DownloadFileTaskAsync(file, path);
        }

        public async Task<string> DownloadStringAsync(string path)
        {
            string result = "";

            await Task.Run(() =>
            {
                result = _web.DownloadString(path);
            });

            return result;
        }

        public void StopDownloadAsync()
        {

        }
    }
}
