namespace Core.CmdLine
{
    /// <summary>
    /// Exception thrown if there is a problem with a command line argument definition.
    /// </summary>
    public class CmdLineArgumentException : CmdLineException
    {
        /// <summary>
        /// Creates an instance of CmdLineArgumentException.
        /// </summary>
        /// <param name="message"></param>
        public CmdLineArgumentException(string message)
            : base(message)
        {
        }
    }
}
