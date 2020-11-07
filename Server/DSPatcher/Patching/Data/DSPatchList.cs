using System;
using System.Collections.Generic;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class DSPatchList
    {
        public DSPatchInfo[] PatchInfos { get; set; }

        public DSPatchList(DSPatchInfo[] patchInfos)
        {
            PatchInfos = patchInfos;
        }

        public DSPatchList()
        {
        }
    }
}
