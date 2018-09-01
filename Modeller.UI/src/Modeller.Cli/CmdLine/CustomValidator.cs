namespace Core.CmdLine
{
    /// <summary>
    /// Base class for custom property validators for the BizArk command-line parser.
    /// </summary>
    public abstract class CustomValidator : ICustomValidator
    {

        /// <summary>
        /// Gets or sets the error message. Include a {0} where you want the name of the property.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the formatted error message.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns></returns>
        public virtual string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name);
        }

        /// <summary>
        /// Checks to make sure the value is correct.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool IsValid(object value);
    }
}
