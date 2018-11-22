using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hy.Modeller.Interfaces;

namespace Hy.Modeller.Outputs
{
    public class FileGroup : IFileGroup
    {
        private readonly IList<IFile> _files = new List<IFile>();

        public IEnumerable<IFile> Files => new ReadOnlyCollection<IFile>(_files);

        public string Path { get; set; }

        public string Name { get; set; }

        public void AddFile(IFile file)
        {
            if (file != null)
                _files.Add(file);
        }
        public IFile AddFile(string content, string path, string name)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            var file = new File { Content = content, Name = name, Path = path };
            _files.Add(file);
            return file;
        }
    }
}