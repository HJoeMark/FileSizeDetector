using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSizeDetector
{
    internal class FolderOrFile
    {
        public FolderOrFile(bool isFolder, string name, string path, long size, List<FolderOrFile>? children)
        {
            IsFolder = isFolder;
            Name = name;
            Path = path;
            Size = size;
            Children = children;
        }

        public bool IsFolder { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; } //bytes
        public List<FolderOrFile>? Children { get; set; }
    }
}
