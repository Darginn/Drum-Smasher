using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoUpdaterDotNET;
using DSPatcher.Patching;
using DSPatcher.Patching.Data;
using Octokit;

namespace DSPatcher
{
    class Program
    {
        public static Logger Logger { get; private set; }

        private const string _OWNER = "Blade12629";
        private const string _REPOSITORY = "Drum-Smasher-Update";
        private const string _PROJECT_NAME = "DrumSmasher";
        private const string _PATCHER_VERSION = "PatcherV0.1";

        private static AsciiLoadingBar _loadingBar;
        private static GithubController _githubController;
        private static DSPatcher _patcher;

        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        private static async Task MainTask(string[] args)
        {
            try
            {
                Logger = new Logger(true);

                DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
                _loadingBar = new AsciiLoadingBar(50, 100, 0, "Patching...");

                DirectoryInfo oldDir = new DirectoryInfo(Path.Combine(currentDirectory.FullName, "old/"));
                DirectoryInfo newDir = new DirectoryInfo(Path.Combine(currentDirectory.FullName, "new/"));

                //CreatePatch(oldDir, newDir, new short[4] { 1, 0, 0, 0 }, "TestUpdate1.0.0.0", "1.0.0.0");
                //ApplyPatch(oldDir);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Done");
            await Task.Delay(-1);
        }

        private static void CreatePatch(DirectoryInfo originalDir, DirectoryInfo newDir, short[] version, string updateName, string versionString)
        {
            _patcher = new DSPatcher(originalDir, newDir, Logger);
            FileInfo patchDest = new FileInfo($"{updateName}.{versionString}");
            PatchInfo pi = _patcher.CreatePatch(new VersionInfo(new short[4] { 1, 0, 0, 0 }), updateName + '/', saveTo: patchDest);

            DSPatchList patchList = _patcher.CreateOrUpdatePatchList(new FileInfo("patch.list"), pi, true);
        }

        private static void ApplyPatch(DirectoryInfo gameFolder)
        {
            _githubController = new GithubController(_OWNER, _REPOSITORY, _PROJECT_NAME, _PATCHER_VERSION);
            _patcher = new DSPatcher(gameFolder, _githubController, Logger);

            PatchInfo pi;
            while ((pi = _patcher.GetNextVersion()) != null)
                _patcher.Patch(pi);
        }
    }
}
