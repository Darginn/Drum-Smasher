using System;
using System.Collections.Generic;
using System.Text;

namespace DSPatcher.Patching.Data
{
    public class DSFileAction
    {
        public FileAction Action { get; set; }
        public string RelativePath { get; set; }
        /// <summary>
        /// Used for <see cref="FileAction.Resize"/>
        /// </summary>
        public long Length { get; set; }

        public DSFileAction(FileAction action, string relativePath)
        {
            Action = action;
            RelativePath = relativePath;
        }
        public DSFileAction(FileAction action, string relativePath, long newLength)
        {
            Action = action;
            RelativePath = relativePath;
            Length = newLength;
        }

        public DSFileAction()
        {
        }
    }

}
