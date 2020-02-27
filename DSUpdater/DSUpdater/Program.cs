//using DSUpdater.Filesystem;
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
        //private static Updater.HTTPUpdater _updater;

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

                DirectoryInfo installDir = new DirectoryInfo(Directory.GetCurrentDirectory());

                //_updater = new Updater.HTTPUpdater();

                //string updaterInfo = await _updater.DownloadStringAsync(updateInfo, routes);

                //if (string.IsNullOrEmpty(updaterInfo))
                //{
                //    Console.WriteLine("No new updates");
                //    await Task.Delay(-1);
                //}

                //List<string> fileDownloads = updaterInfo.Split(Environment.NewLine).ToList();
                //Dictionary<string, string> checkSums = new Dictionary<string, string>();

                //bool cont = false;
                //for (int i = 0; i < fileDownloads.Count; i++)
                //{
                //    if (fileDownloads[i].Equals(@"/\"))
                //    {
                //        cont = true;
                //        continue;
                //    }

                //    if (!cont)
                //        continue;

                //    string[] split = fileDownloads[i].Split('=');

                //    checkSums.Add(split[0], split[1]);
                //    i--;
                //}

                //IEnumerable<FileChecksum> invalidFiles = FileChecksum.CheckAgainst(installDir.FullName, checkSums);

                //if (invalidFiles.Count() > 0)
                //{

                //}

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
