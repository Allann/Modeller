using Modeller.Interfaces;

namespace Modeller.Outputs
{
    public class File : IFile
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string FullName
        {
            get
            {
                var path = string.IsNullOrWhiteSpace(Path) ? string.Empty : Path;
                var name = string.IsNullOrWhiteSpace(Name) ? "NotNamed" : Name;
                return System.IO.Path.Combine(path, name);
            }
        }

        public bool CanOverwrite { get; set; }
    }

    public class FileCopy : IFileCopy
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Name { get; }
    }

    public class FolderCopy : IFolderCopy
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool IncludeSubFolders { get; set; } = true;
        public string Name { get; }
    }
}