using System;
using System.Collections.Generic;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class DSPatchInfo
    {
        public VersionInfo Version { get; set; }
        public string GitPatchFilePath { get; set; }

        public DSPatchInfo(VersionInfo version, string gitPatchFilePath)
        {
            Version = version;
            GitPatchFilePath = gitPatchFilePath;
        }

        public DSPatchInfo()
        {
        }
    }
}
