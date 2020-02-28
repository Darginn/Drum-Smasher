//using DSUpdater.Filesystem;
using DSUpdater.Updater;
using DSUpdater.Updater.Filesystem;
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
        //private static Updater.HTTPUpdater _updater;
        private static HTTPDownloader _downloader;

        static async Task MainTask(string[] args)
        {
            try
            {
                //Dictionary<string, string> ddlList = new Dictionary<string, string>()
                //{
                //    { @"\test\Camellia - Nacreous Snowmelt (rubies87).rar", "http://puu.sh/Ffpw9/bd77c86b20.rar" },
                //    { @"\test\DM DOKURO - Devourer of Gods Compilation (Stefan).rar", "http://puu.sh/FfpwH/e48831a5b9.rar" },
                //    { @"\Infected Mushroom - The Messenger 2012 (Sped Up Ver.) (Nwolf).rar", "http://puu.sh/Ffpx6/af65c16088.rar" },
                //    { @"\katagiri - Sendan Life (katagiri Bootleg) (Ozu).rar", "http://puu.sh/FfpxQ/fd001bc889.rar" },
                //    { @"\Kola Kid - can't hide your love (Kert).rar", "http://puu.sh/FfpxQ/fd001bc889.rar" },
                //    { @"Masayoshi Minoshima feat. nomico - Bad Apple!! (ouranhshc).rar", "http://puu.sh/Ffpyx/2a79059d68.rar" },
                //    { @"SoundTeMP - Dreamer's Dream (P A N).rar", "http://puu.sh/FfpyB/3ed0efd8b0.rar" },
                //    { @"toby fox - MEGALOVANIA (Camellia Remix) (Rhytoly).rar", "http://puu.sh/Ffpzr/e74ae08d39.rar" },
                //    { @"Xi - Majotachi no Butoukai ~ Magus (Kite).rar", "http://puu.sh/FfpzQ/567924e894.rar" },
                //    { @"Yuuyu - Howdy! and... Good-Die! (3san).rar", "http://puu.sh/FfpA0/c084bc4a7f.rar" },
                //};

                //DirectoryInfo folder = new DirectoryInfo(@"C:\Games\UpdaterTest\Charts\");

                //Dictionary<string, string> fileCheckSums = new Dictionary<string, string>();

                //foreach (FileInfo f in folder.EnumerateFiles())
                //{
                //    string path = f.FullName.Remove(0, folder.FullName.Length);
                //    FileChecksum fc = new FileChecksum(f.FullName);

                //    fileCheckSums.Add(path, fc.Checksum);
                //}

                //UpdateInfo upInfo = new UpdateInfo("http://puu.sh/");
                //upInfo.DownloadList = ddlList;
                //upInfo.ChecksumList = fileCheckSums;
                //upInfo.Version = new int[4]
                //{
                //    0,
                //    0,
                //    0,
                //    1
                //};

                //string upInfoJson = upInfo.ToString();

                //File.WriteAllText(Path.Combine(folder.FullName, @"upInfo.txt"), upInfoJson);

                //Console.WriteLine("done, " + Path.Combine(folder.FullName, @"upInfo.txt"));
                //await Task.Delay(-1);

                //string updateUrl = "https://raw.githubusercontent.com";
                //string[] routes = new string[]
                //{
                //    "Darginn",
                //    "Drum-Smasher",
                //    "master",
                //    "upinfo.txt"
                //};
                //http://puu.sh/FfpIC/122ad8d03a.txt

                File.Delete(@"C:\Games\UpdaterTest\Charts\Install\updateinfo.json");

                string updateUrl = "http://puu.sh";
                string[] routes = new string[]
                {
                    "FfpIC",
                    "122ad8d03a.txt"
                };

                _downloader = new HTTPDownloader(updateUrl);

                string updateInfo = updateUrl;

                foreach (string route in routes)
                    updateInfo += "/" + route;

                string updateJson = await _downloader.DownloadStringAsync(updateInfo);
                UpdateInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInfo>(updateJson);

                if (info == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Failed to get update info");
                    Console.ForegroundColor = ConsoleColor.White;

                    await Task.Delay(-1);
                }

                //DirectoryInfo installDir = new DirectoryInfo(Directory.GetCurrentDirectory());                
                DirectoryInfo installDir = new DirectoryInfo(@"C:\Games\UpdaterTest\Charts\Install\");

                FileInfo lastUpdateInfo = new FileInfo(Path.Combine(installDir.FullName, @"updateinfo.json"));

                if (lastUpdateInfo.Exists)
                {
                    UpdateInfo lastUpdateJson = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInfo>(File.ReadAllText(lastUpdateInfo.FullName));

                    bool pass = true;
                    for (int i = 0; i < lastUpdateJson.Version.Length; i++)
                    {
                        int lastVer = lastUpdateJson.Version[i];
                        int curVer = info.Version[i];

                        if (lastVer != curVer)
                        {
                            pass = false;
                            break;
                        }
                    }

                    if (pass)
                    {
                        Console.WriteLine("We are up to date");
                        await Task.Delay(-1);
                    }
                }

                File.WriteAllText(lastUpdateInfo.FullName, updateJson);
                Console.WriteLine("Updating...");

                _downloader.DownloadDict = info.DownloadList;
                Console.WriteLine("Starting download");

                _downloader.StartDownloadThreaded(installDir.FullName, 4);

                while(_downloader.ToDownload > 0)
                {
                    Console.WriteLine("Files to download left: " + _downloader.ToDownload);

                    await Task.Delay(2000);
                }

                Console.WriteLine("Finished downloading, failed files: " + _downloader.FailedDownloads);

                FolderChecksum checksums = new FolderChecksum(installDir.FullName);
                checksums.GenerateChecksums();

                foreach(var pair in info.ChecksumList)
                {
                    FileChecksum f = checksums.Files.FirstOrDefault(fc => Path.Combine(fc.Folder, fc.File).Contains(pair.Key));

                    if (f == null || string.IsNullOrEmpty(f.File))
                    {
                        Console.WriteLine("Could not find checksum for: " + pair.Key);
                        continue;
                    }

                    if (!f.Checksum.Equals(pair.Value))
                        Console.WriteLine("File checksum invalid: " + pair.Key);
                    else
                        Console.WriteLine("File is valid: " + pair.Key);
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
