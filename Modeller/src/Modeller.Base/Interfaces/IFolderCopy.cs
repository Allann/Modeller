namespace Hy.Modeller.Interfaces
{
    /// <summary>
    /// An output type that represents a folder to copy.  No changes will be made to the files.
    /// </summary>
    public interface IFolderCopy : IOutput
    {
        /// <summary>
        /// The source folder name
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// The destination folder name
        /// </summary>
        string Destination { get; set; }

        /// <summary>
        /// Flag whether to include sub folders
        /// </summary>
        bool IncludeSubFolders { get; set; }
    }
}