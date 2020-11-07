using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Security.Cryptography;
using DSPatcher.Patching.Data;
using System.IO;
using System.Linq;
using DSPatcher.Patching;

namespace DSPatcher
{
    public class DSPatcher
    {
        public DirectoryInfo GameDir { get; set; }
        public DirectoryInfo PatchDir { get; set; }
        public GithubController GithubController
        {
            get
            {
                return _git;
            }
            set
            {
                _git = value;
            }
        }

        public Logger Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        private Logger _logger;
        private GithubController _git;
        private string _baseGithubPath;

        public DSPatcher(DirectoryInfo gameDir, GithubController controller, Logger logger)
        {
            GameDir = gameDir;
            GithubController = controller;
            Logger = logger;
        }

        public DSPatcher(DirectoryInfo gameDir, DirectoryInfo patchDir, Logger logger)
        {
            GameDir = gameDir;
            PatchDir = patchDir;
            _logger = logger;
        }

        public void Patch(PatchInfo patchInfo)
        {
            _baseGithubPath = patchInfo.GithubPatchDirectory;

            if (_baseGithubPath[_baseGithubPath.Length - 1] != '/')
                _baseGithubPath += '/';

            for (int i = 0; i < patchInfo.Actions.Length; i++)
            {
                switch(patchInfo.Actions[i].Action)
                {
                    case FileAction.Delete:
                        FileInfo deleteFile = new FileInfo(Path.Combine(GameDir.FullName, patchInfo.Actions[i].RelativePath));

                        if (deleteFile.Exists)
                            deleteFile.Delete();
                        break;

                    case FileAction.Create:
                        byte[] file = GetGithubFile(patchInfo.Actions[i].RelativePath);

                        File.WriteAllBytes(Path.Combine(GameDir.FullName, patchInfo.Actions[i].RelativePath), file);
                        break;

                    case FileAction.Resize:
                        FileInfo resizeFile = new FileInfo(Path.Combine(GameDir.FullName, patchInfo.Actions[i].RelativePath));

                        using (FileStream fstream = resizeFile.OpenWrite())
                            fstream.SetLength(patchInfo.Actions[i].Length);
                        break;
                }
            }

            using (MD5 md = MD5.Create())
            {
                for (int i = 0; i < patchInfo.Files.Length; i++)
                {
                    FileInfo file = new FileInfo(Path.Combine(GameDir.FullName, patchInfo.Files[i].RelativePath));

                    using (FileStream fstream = (file.Exists ? file.OpenWrite() : file.Create()))
                    {
                        for (int x = 0; x < patchInfo.Files[i].ChunkInfos.Length; x++)
                        {
                            DSChunkInfo dsChunk = patchInfo.Files[i].ChunkInfos[x];

                            if (fstream.Length < dsChunk.Start + dsChunk.Length)
                            {
                                ApplyChunk(fstream, $"{patchInfo.Files[i].RelativePath}.{dsChunk.Start}.{dsChunk.Length}", dsChunk.Start);
                                continue;
                            }

                            byte[] chunkData = new byte[dsChunk.Length];

                            fstream.Position = dsChunk.Start;
                            fstream.Read(chunkData, 0, chunkData.Length);

                            byte[] checksum = md.ComputeHash(chunkData);

                            if (!IsArrayEqual(dsChunk.Checksum, checksum))
                                ApplyChunk(fstream, $"{patchInfo.Files[i].RelativePath}.{dsChunk.Start}.{dsChunk.Length}", dsChunk.Start);
                        }

                        if (fstream.Length > patchInfo.Files[i].FileLength)
                            fstream.SetLength(patchInfo.Files[i].FileLength);

                        fstream.Flush();
                    }
                }
            }

            bool IsArrayEqual(byte[] a, byte[] b)
            {
                if (a == null || b == null ||
                    a.Length != b.Length)
                    return false;

                for (int i = 0; i < a.Length; i++)
                    if (a != b)
                        return false;

                return true;
            }

            void ApplyChunk(FileStream _fstream, string gitPath, long start)
            {
                byte[] gitChunk = GetGithubFile(gitPath);

                _fstream.Position = start;
                _fstream.Write(gitChunk, 0, gitChunk.Length);
                _fstream.Flush();
            }
        }

        public PatchInfo GetNextVersion()
        {
            FileInfo versionFile = new FileInfo(Path.Combine(GameDir.FullName, "version.info"));
            
            VersionInfo currentVersion;
            if (!versionFile.Exists)
                currentVersion = new VersionInfo(4);
            else
            {
                string json = File.ReadAllText(versionFile.FullName);
                currentVersion = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionInfo>(json);
            }

            DSPatchList patchList = Newtonsoft.Json.JsonConvert.DeserializeObject<DSPatchList>(GetGithubString("version.list"));
            DSPatchInfo patchInfo = patchList.PatchInfos.FirstOrDefault(pi => pi.Version > currentVersion);

            if (patchInfo == null)
                return null;

            PatchInfo patch = Newtonsoft.Json.JsonConvert.DeserializeObject<PatchInfo>(GetGithubString(patchInfo.GitPatchFilePath));

            return patch;
        }

        private string GetGithubString(string path)
        {
            return Encoding.UTF8.GetString(GetGithubFile(path));
        }

        private byte[] GetGithubFile(string path)
        {
            return GithubController.GetContent(_baseGithubPath + path).Result;
        }

        public DSPatchList CreateOrUpdatePatchList(FileInfo patchFile, PatchInfo patch, bool save = true)
        {
            DSPatchList patchList;

            if (!patchFile.Exists)
                patchList = new DSPatchList(new DSPatchInfo[0]);
            else
                patchList = Newtonsoft.Json.JsonConvert.DeserializeObject<DSPatchList>(File.ReadAllText(patchFile.FullName));

            List<DSPatchInfo> patchInfos = patchList.PatchInfos.ToList();
            patchInfos.Add(new DSPatchInfo(patch.Version, $"{patch.GithubPatchDirectory.TrimEnd('/', '\\')}.{patch.Version.ToString()}"));

            patchList.PatchInfos = patchInfos.ToArray();

            if (save)
            {
                if (patchFile.Exists)
                    patchFile.Delete();

                File.WriteAllText(patchFile.FullName, Newtonsoft.Json.JsonConvert.SerializeObject(patchList, Newtonsoft.Json.Formatting.Indented));
            }

            return patchList;
        }

        /// <param name="saveTo">Format: "{GithubPatchDirectory}.{PatchVersion}"</param>
        /// <returns></returns>
        public PatchInfo CreatePatch(VersionInfo version, string githubPatchPath, List<DSFileInfo> dsFiles = null, List<DSFileAction> dsActions = null, FileInfo saveTo = null)
        {
            if (dsActions == null)
                dsActions = CreatePatchActions();

            if (dsFiles == null)
            {
                var patchFilesAndActions = CreatePatchFiles();

                if (patchFilesAndActions.Item1 != null)
                {
                    dsFiles = new List<DSFileInfo>();
                    dsFiles.AddRange(patchFilesAndActions.Item1);
                }

                if (patchFilesAndActions.Item2 != null)
                {
                    if (dsActions == null)
                        dsActions = new List<DSFileAction>();

                    dsActions.AddRange(patchFilesAndActions.Item2);
                }
            }

            PatchInfo pi = new PatchInfo(version, dsFiles?.ToArray() ?? null, dsActions?.ToArray() ?? null, githubPatchPath);

            if (saveTo != null)
            {
                if (saveTo.Exists)
                    saveTo.Delete();

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(pi, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(saveTo.FullName, json);
            }

            return pi;
        }

        public List<DSFileAction> CreatePatchActions()
        {
            List<FileInfo> patchDirFiles = GetFiles(PatchDir);
            List<DSFileAction> actions = new List<DSFileAction>();

            for (int i = 0; i < patchDirFiles.Count; i++)
            {
                string relPath = Path.GetRelativePath(PatchDir.FullName, patchDirFiles[i].FullName);
                FileInfo gameFile = new FileInfo(Path.Combine(GameDir.FullName, relPath));

                if (gameFile.Exists)
                    continue;

                actions.Add(new DSFileAction(FileAction.Create, relPath));
            }

            List<FileInfo> gameDirFiles = GetFiles(GameDir);

            for (int i = 0; i < gameDirFiles.Count; i++)
            {
                string relPath = Path.GetRelativePath(GameDir.FullName, gameDirFiles[i].FullName);
                FileInfo patchFile = new FileInfo(Path.Combine(PatchDir.FullName, relPath));

                if (patchFile.Exists)
                    continue;

                actions.Add(new DSFileAction(FileAction.Delete, relPath));
            }

            if (actions.Count == 0)
                return null;

            return actions;
        }

        /// <summary>
        /// Creates patch files for all changed files in <see cref="GameDir"/> and <see cref="PatchDir"/>
        /// </summary>
        /// <returns>null if none otherwise list</returns>
        public (List<DSFileInfo>, List<DSFileAction>) CreatePatchFiles()
        {
            List<FileInfo> patchDirFiles = GetFiles(PatchDir);
            List<DSFileInfo> patchFiles = new List<DSFileInfo>();
            List<DSFileAction> patchActions = new List<DSFileAction>();

            for (int i = 0; i < patchDirFiles.Count; i++)
            {
                _logger.Log($"Checking file {patchDirFiles[i].FullName}");

                FileInfo mirror = FindGameDirFile(patchDirFiles[i]);

                if (!mirror.Exists)
                {
                    _logger.Log($"Could not find mirror {mirror.FullName}", LogLevel.Warning);
                    continue;
                }

                List<DSChunkInfo> diffs = GetDiffrences(mirror, patchDirFiles[i]);

                if (diffs == null)
                {
                    if (patchDirFiles[i].Length < mirror.Length)
                    {
                        _logger.Log($"No diffrences found but found resize action");
                        patchActions.Add(new DSFileAction(FileAction.Resize, Path.GetRelativePath(GameDir.FullName, mirror.FullName), patchDirFiles[i].Length));
                        continue;
                    }

                    _logger.Log($"No diffrences found");
                    continue;
                }

                _logger.Log($"Found {diffs.Count} diffrences");

                string relPath = Path.GetRelativePath(PatchDir.FullName, patchDirFiles[i].FullName);
                DSFileInfo fileInfo = new DSFileInfo(relPath, diffs.ToArray(), patchDirFiles[i].Length);

                patchFiles.Add(fileInfo);
            }

            if (patchFiles.Count == 0)
                patchFiles = null;
            if (patchActions.Count == 0)
                patchActions = null;

            return (patchFiles, patchActions);
        }

        /// <summary>
        /// Gets the same file from <see cref="GameDir"/>
        /// </summary>
        private FileInfo FindGameDirFile(FileInfo patchDirFile)
        {
            string relativePath = Path.GetRelativePath(PatchDir.FullName, patchDirFile.FullName);

            return new FileInfo(Path.Combine(GameDir.FullName, relativePath));
        }

        /// <summary>
        /// Gets the same file from <see cref="PatchDir"/>
        /// </summary>
        private FileInfo FindPatchDirFile(FileInfo gameDirFile)
        {
            string relativePath = Path.GetRelativePath(GameDir.FullName, gameDirFile.FullName);

            return new FileInfo(Path.Combine(PatchDir.FullName, relativePath));
        }

        /// <summary>
        /// Gets all files in a directory recursive
        /// </summary>
        /// <returns>null if none otherwise list</returns>
        private List<FileInfo> GetFiles(DirectoryInfo directory)
        {
            List<FileInfo> files = new List<FileInfo>();
            List<DirectoryInfo> directories = new List<DirectoryInfo>()
            {
                directory
            };

            for (int i = 0; i < directories.Count; i++)
            {
                files.AddRange(directories[i].EnumerateFiles());
                directories.AddRange(directories[i].EnumerateDirectories());
            }

            if (files.Count == 0)
                return null;

            return files;
        }

        /// <summary>
        /// Gets the diffrences between two <see cref="FileInfo"/>s
        /// </summary>
        /// <returns>null if none otherwise list</returns>
        private List<DSChunkInfo> GetDiffrences(FileInfo orig, FileInfo @new)
        {
            using (FileStream origStream = orig.OpenRead())
            {
                using (FileStream newStream = @new.OpenRead())
                {
                    return GetDiffrences(origStream, newStream);
                }
            }
        }

        /// <summary>
        /// Gets the diffrences between two <see cref="FileStream"/>s
        /// </summary>
        /// <returns>null if none otherwise list</returns>
        private List<DSChunkInfo> GetDiffrences(FileStream orig, FileStream @new, bool saveChunks = true)
        {
            List<byte> chunkData = new List<byte>();
            List<DSChunkInfo> chunks = new List<DSChunkInfo>();
            long pos = 0;

            while (@new.Position < @new.Length)
            {
                byte newb = (byte)@new.ReadByte();

                if (orig.Length < @new.Position)
                {
                    if (chunkData.Count == 0)
                        pos = @new.Position - 1;

                    chunkData.Add(newb);
                    continue;
                }

                byte origb = (byte)orig.ReadByte();

                if (origb != newb)
                {
                    if (chunkData.Count == 0)
                        pos = @new.Position - 1;

                    chunkData.Add(newb);
                }
                else if (chunkData.Count > 0)
                    Submit();
            }

            if (chunkData.Count > 0)
                Submit();

            if (chunks.Count == 0)
                return null;

            return chunks;

            void Submit()
            {
                if (chunkData.Count > 0)
                {
                    _logger.Log("Submitting chunk");

                    DSChunkInfo chunk = DSChunkInfo.FromBytes(chunkData.ToArray(), pos);

                    if (saveChunks)
                    {
                        FileInfo file = new FileInfo($"{orig.Name}.{chunk.Start}.{chunk.Length}");

                        _logger.Log($"Saving chunk to {file.FullName}");

                        if (file.Exists)
                            file.Delete();

                        using (FileStream fstream = file.Create())
                        {
                            for (int i = 0; i < chunkData.Count; i++)
                                fstream.WriteByte(chunkData[i]);

                            fstream.Flush();
                        }
                    }

                    chunks.Add(chunk);
                    chunkData.Clear();
                }
            }
        }
    }
}
