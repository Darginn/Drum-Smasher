using DSUpdater.Updater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DSUpdater
{
    class Program
    {
        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();


        private static bool _closing;
        private static Updater.HTTPUpdater _updater;

        static async Task MainTask(string[] args)
        {
            try
            {
                string updateInfo = "https://raw.githubusercontent.com";
                string[] routes = new string[]
                {
                    "Darginn",
                    "Drum-Smasher",
                    "master",
                    "upinfo.txt"
                };

                _updater = new Updater.HTTPUpdater();

                string upInfo = await _updater.DownloadStringAsync(updateInfo, routes);

                if (string.IsNullOrEmpty(upInfo))
                {
                    Console.WriteLine("No updates found");
                    await Task.Delay(-1);
                }

                List<string> info = upInfo.Split(Environment.NewLine.ToCharArray()).ToList();

                string[] version = info[0].Split('.');
                string ver = version[0];
                string comp = version[1];
                string run = version[2];

                //file, (file, filepath, checksum)
                Dictionary<string, (string, string[], string)> fileChecksums = new Dictionary<string, (string, string[], string)>();
                List<(FileInfo, string, string[])> failedFiles = new List<(FileInfo, string, string[])>();

                for (int i = 3; i < info.Count; i++)
                {
                    if (info[i].StartsWith("!create"))
                    {
                        string toCreate = info[i].Remove(0, "!create ".Length);

                        DirectoryInfo dir = new DirectoryInfo(toCreate);

                        dir.Create();

                        continue;
                    }

                    string[] fcsS = info[i].Split('=');
                    string checksum = fcsS[1];

                    fcsS = fcsS[0].Split('/');

                    fileChecksums.Add(fcsS[fcsS.Length - 1], (fcsS[fcsS.Length - 1], fcsS, checksum));
                }

                DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

                foreach(FileInfo fi in currentDir.EnumerateFiles())
                {
                    string fiName = fi.Name + fi.Extension;

                    Console.WriteLine("Checking " + fiName);

                    if (!fileChecksums.ContainsKey(fi.Name + fi.Extension))
                    {
                        RequestToDelete(fi.Name);
                        continue;
                    }

                    FileChecksum fc = new FileChecksum(fi.FullName);

                    if (fc.Checksum.Equals(fileChecksums[fiName].Item3))
                    {
                        Console.WriteLine($"{fiName} passed checksum pass");

                        fileChecksums.Remove(fiName);
                        continue;
                    }

                    failedFiles.Add((fi, fiName, fileChecksums[fiName].Item2));
                    Console.WriteLine($"{fiName} failed checksum pass (total: {failedFiles.Count})");

                    fileChecksums.Remove(fiName);
                }

                failedFiles.ForEach(f => fileChecksums.Add(f.Item1.Name + f.Item1.Extension, (f.Item1.Name + f.Item1.Extension, f.Item3, f.Item2)));
                
                foreach(var pair in fileChecksums.Values)
                {
                    using (FileStream fstream = new FileStream(pair.Item1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        byte[] data = await _updater.DownloadAsync("host", pair.Item2);
                        fstream.Write(data, 0, data.Length);
                    }

                    FileChecksum fc = new FileChecksum(pair.Item1);

                    if (!fc.Checksum.Equals(pair.Item3))
                    {
                        Console.WriteLine("checksum wrong after download");
                        return;
                    }
                }

                Console.WriteLine("Finished all processes!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(-1);
        }

        public static void RequestToDelete(string file)
        {
            FileInfo fi = new FileInfo(file);

            if (!fi.Exists)
                return;

            Console.WriteLine($"Please confirm if {fi.Name} should be deleted");
            Console.WriteLine("Press 'y' for yes and 'n' for no");

            char result = GetNextChar(new char[] { 'y', 'Y', 'n', 'N' });

            if (result.Equals('y') || result.Equals('Y'))
            {
                fi.Delete();
                Console.WriteLine($"{fi.Name + fi.Extension} deleted");
            }
            else
                Console.WriteLine("Did not delete " + fi.Name + fi.Extension);
        }

        private static char GetNextChar(params char[] options)
        {
            char result = char.MinValue;

            while (result == char.MinValue || !options.Contains(result))
                result = Console.ReadKey().KeyChar;

            return result;
        }
    }
}
