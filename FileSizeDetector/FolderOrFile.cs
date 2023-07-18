using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSizeDetector
{
    internal class FolderOrFile
    {
        public FolderOrFile(bool isFolder, string path, long size, List<FolderOrFile>? children)
        {
            IsFolder = isFolder;
            Path = path;
            Size = size;
            Children = children;
        }

        public bool IsFolder { get; set; }       
        public string Path { get; set; }
        public long Size { get; set; } //bytes
        public List<FolderOrFile>? Children { get; set; }
    }
}
