using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class PatchInfo : IEquatable<PatchInfo>
    {
        public VersionInfo Version { get; set; }
        public DSFileInfo[] Files { get; set; }
        public DSFileAction[] Actions { get; set; }
        public string GithubPatchDirectory { get; set; }

        public PatchInfo(VersionInfo version, DSFileInfo[] files, DSFileAction[] actions, string githubPatchDirectory)
        {
            Version = version;
            Files = files;
            Actions = actions;
            GithubPatchDirectory = githubPatchDirectory;
        }

        public PatchInfo()
        {
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PatchInfo);
        }

        public bool Equals([AllowNull] PatchInfo other)
        {
            return other != null &&
                   EqualityComparer<VersionInfo>.Default.Equals(Version, other.Version) &&
                   EqualityComparer<DSFileInfo[]>.Default.Equals(Files, other.Files) &&
                   EqualityComparer<DSFileAction[]>.Default.Equals(Actions, other.Actions) &&
                   GithubPatchDirectory == other.GithubPatchDirectory;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version, Files, GithubPatchDirectory);
        }

        public static bool operator ==(PatchInfo left, PatchInfo right)
        {
            return EqualityComparer<PatchInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(PatchInfo left, PatchInfo right)
        {
            return !(left == right);
        }
    }

}
