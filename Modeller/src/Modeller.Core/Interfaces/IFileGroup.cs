
using System.Collections.Generic;

namespace Hy.Modeller.Interfaces
{
    /// <summary>
    /// An output type that represents a group of generated files
    /// </summary>
    public interface IFileGroup : IOutput
    {
        /// <summary>
        /// A collection of generated files
        /// </summary>
        IEnumerable<IFile> Files { get; }

        /// <summary>
        /// The path used by the <see cref="IFileGroup"/>
        /// </summary>
        string Path { get; set; }

        void AddFile(IFile file);

        IFile AddFile(string content, string path, string name);
    }
}