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

        public UpdateInfo(string host)
        {
            Host = host;

            DownloadList = new Dictionary<string, string>();
            ChecksumList = new Dictionary<string, string>();
            Version = new int[4];
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
