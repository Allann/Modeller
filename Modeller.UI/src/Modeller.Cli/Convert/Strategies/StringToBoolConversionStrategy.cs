using System;
using System.Collections.Generic;

namespace Core.Convert.Strategies
{
    /// <summary>
    /// Converts from a string to a bool.
    /// </summary>
    public class StringToBoolConversionStrategy
        : IConvertStrategy
    {
        static StringToBoolConversionStrategy()
        {
            TrueValues = new List<string>() { "true", "t", "yes", "ok" };
        }

        /// <summary>
        /// Gets the list of values that will equate to True. Everything else is false.
        /// </summary>
        public static List<string> TrueValues { get; private set; }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(Type from, Type to, object value, IFormatProvider provider)
        {
            if (!(value is string strVal))
                return false;

            if (int.TryParse(strVal, out var i))
                return i != 0; // only false if i == 0.

            foreach (var trueVal in TrueValues)
            {
                if (trueVal.Equals(strVal, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            // Everything else is false.
            return false;
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            return from != typeof(string) ? false : to != typeof(bool) ? false : true;
        }
    }
}
