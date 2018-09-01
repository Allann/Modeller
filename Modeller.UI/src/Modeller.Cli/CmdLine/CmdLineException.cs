using System;

namespace Core.CmdLine
{
    /// <summary>
    /// Base class for exceptions used for command-line parsing.
    /// </summary>
    public class CmdLineException : ApplicationException
    {

        /// <summary>
        /// Creates an instance of CmdLineException.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CmdLineException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
