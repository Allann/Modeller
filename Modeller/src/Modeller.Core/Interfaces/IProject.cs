using System;
using System.Collections.Generic;

namespace Modeller.Interfaces
{
    /// <summary>
    /// Defines the files to be used as a project
    /// </summary>
    public interface IProject : IOutput
    {
        /// <summary>
        /// The files within the project that should be generated
        /// </summary>
        IEnumerable<IFileGroup> FileGroups { get; }

        /// <summary>
        /// The files within the project that should be copied
        /// </summary>
        IEnumerable<IFolderCopy> Folders { get; }

        /// <summary>
        /// Relative path if used within a <see cref="ISolution"/> otherwise a full path
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Project Id as used by Visual Studio
        /// </summary>
        Guid Id { get; set; }

        void AddFileGroup(IFileGroup file);

        void AddFolder(IFolderCopy folder);
    }
}