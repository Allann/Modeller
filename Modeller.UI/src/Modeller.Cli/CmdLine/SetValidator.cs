using System.Collections;
using System.Linq;

namespace Core.CmdLine
{
    /// <summary>
    /// Ensures the value is contained within the set of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetValidator<T> : CustomValidator
    {
        /// <summary>
        /// Creates an instance of SetValidator.
        /// </summary>
        /// <param name="values"></param>
        public SetValidator(params T[] values)
            : this(null, values)
        {
        }

        /// <summary>
        /// Creates an instance of SetValidator.
        /// </summary>
        /// <param name="comparer">The comparer to use to compare the values.</param>
        /// <param name="values"></param>
        public SetValidator(IEqualityComparer comparer, params T[] values)
        {
            Comparer = comparer;
            ErrorMessage = "The field {0} must be one of these values: {1}.";
            Values = values;
        }

        /// <summary>
        /// Gets the comparer to use to compare the values. If not set, uses Values[x].Equals(value).
        /// </summary>
        public IEqualityComparer Comparer { get; private set; }

        /// <summary>
        /// Gets the set of valid values.
        /// </summary>
        public T[] Values { get; private set; }

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            var displayVals = Values.Cast<string>();
            return string.Format(ErrorMessage,name, string.Join(", ", displayVals));
        }

        /// <summary>
        /// Checks that the value of the data field is valid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null) return true; // Use RequiredAttribute to validate for a value.

            foreach (var validVal in Values)
            {
                if (Comparer != null)
                {
                    if (Comparer.Equals(validVal, value))
                        return true;
                }
                else if (validVal.Equals(value)) 
                    return true;
            }
            return false;
        }
    }
}
