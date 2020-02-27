using System;
using System.Collections.Generic;
using System.Text;
using DSUpdater.Updater.Filesystem;

namespace DSUpdater.Updater
{
    public class UpdateInfo
    {
        /// <summary>
        /// Relative File Location, URL
        /// </summary>
        public Dictionary<string, string> DownloadList;
        /// <summary>
        /// Relative File Location, Checksum
        /// </summary>
        public Dictionary<string, string> ChecksumList;
        public int[] Version;

        public string Host;
        public string HostBasePath;

        public UpdateInfo(string host, string hostBasePath)
        {
            Host = host;
            HostBasePath = hostBasePath;

            DownloadList = new Dictionary<string, string>();
            ChecksumList = new Dictionary<string, string>();
            Version = new int[4];
        }
    }
}
