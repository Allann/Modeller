namespace Modeller.Interfaces
{
    /// <summary>
    /// An output type that represents a file to copy.  No changes will be made to this output type.
    /// </summary>
    public interface IFileCopy : IOutput
    {
        /// <summary>
        /// The source file name
        /// </summary>
        string Source { get; set; }

        /// <summary>
        /// The destination file name
        /// </summary>
        string Destination { get; set; }
    }
}