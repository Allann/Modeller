using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hy.Modeller.Interfaces;

namespace Hy.Modeller.Outputs
{
    public class Project : IProject
    {
        private readonly IList<IFileGroup> _fileGroups = new List<IFileGroup>();
        private readonly IList<IFolderCopy> _folders = new List<IFolderCopy>();

        public Project(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("message", nameof(name));
            Name = name;
        }

        public Guid Id { get; set; }

        public IEnumerable<IFileGroup> FileGroups => new ReadOnlyCollection<IFileGroup>(_fileGroups);

        public IEnumerable<IFolderCopy> Folders => new ReadOnlyCollection<IFolderCopy>(_folders);

        public string Path { get; set; }

        public string Name { get; }

        public void AddFileGroup(IFileGroup fileGroup)
        {
            if (fileGroup == null)
                return;
            _fileGroups.Add(fileGroup);
        }

        public void AddFolder(IFolderCopy folder)
        {
            if (folder == null)
                return;
            _folders.Add(folder);
        }
    }
}