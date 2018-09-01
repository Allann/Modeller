namespace Core.CmdLine
{
    /// <summary>
    /// Interface for custom property validators for the BizArk command-line parser.
    /// </summary>
    public interface ICustomValidator
    {

        /// <summary>
        /// Gets or sets the error message. Include a {0} where you want the name of the property.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the formatted error message.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns></returns>
        string FormatErrorMessage(string name);

        /// <summary>
        /// Checks to make sure the value is correct.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsValid(object value);

    }
}
