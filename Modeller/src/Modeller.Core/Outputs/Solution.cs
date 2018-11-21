using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hy.Modeller.Interfaces;

namespace Hy.Modeller.Outputs
{
    public class Solution : ISolution
    {
        private IList<IFile> _files = new List<IFile>();
        public IEnumerable<IFile> Files => new ReadOnlyCollection<IFile>(_files);

        public string Directory { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        private IList<IProject> _projects = new List<IProject>();

        public IEnumerable<IProject> Projects => new ReadOnlyCollection<IProject>(_projects);

        public void AddProject(IProject project) => _projects.Add(project);

        public IProject AddProject(Guid id, string path, string name) => new Project(name) { Id = id, Path = path };

        public void AddFile(IFile file) => _files.Add(file);

        public IFile AddFile(string content, string path, string name) => new File { Content = content, Name = name, Path = path };
    }
}