using System;

namespace Modeller
{
    public class MissingTargetException : ApplicationException
    {
        public string Target { get; }

        public MissingTargetException(string target)
        {
            Target = target;
        }

        public MissingTargetException(string target, string message) : base(message)
        {
            Target = target;
        }

        public MissingTargetException(string target, string message, Exception innerException) : base(message, innerException)
        {
            Target = target;
        }
    }
}
