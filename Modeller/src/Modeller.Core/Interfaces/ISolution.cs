using System;
using System.Collections.Generic;

namespace Modeller.Interfaces
{
    public interface ISolution : IOutput
    {
        string Directory { get; set; }

        IEnumerable<IFile> Files { get; }

        IEnumerable<IProject> Projects { get; }

        void AddFile(IFile file);

        IFile AddFile(string content, string path, string name);

        void AddProject(IProject project);

        IProject AddProject(Guid id, string path, string name);
    }
}