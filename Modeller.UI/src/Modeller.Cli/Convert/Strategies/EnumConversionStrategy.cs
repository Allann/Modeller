using System;
using Core.Extensions;

namespace Core.Convert.Strategies
{
    /// <summary>
    /// Converts to enumeration values.
    /// </summary>
    public class EnumConversionStrategy
        : IConvertStrategy
    {

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
            var cfrom = from.GetTrueType();
            var cto = to.GetTrueType();
            var val = GetTrueValue(value);

            if (cto.IsEnum)
            {
                return val is string s ? Enum.Parse(cto, s) : Enum.ToObject(cto, val);
            }
            else if (cfrom.IsEnum)
                return System.Convert.ChangeType(val, cto);
            else
                throw new InvalidCastException(string.Format("Cannot convert from {0} to {1}.", from.Name, to.Name));

        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            var cfrom = from.GetTrueType();
            var cto = to.GetTrueType();

            return cfrom.IsEnum && (cto.IsNumericType() || cto == typeof(string))
                ? true
                : cto.IsEnum && (cfrom.IsNumericType() || cfrom == typeof(string)) ? true : false;
        }

        private static object GetTrueValue(object val)
        {
            // I'm not sure if this method is necessary. It seems that nullable values
            // are sent in with the actual value or null, not as Nullable<?>.

            if (val == null) return null;
            if (!val.GetType().IsDerivedFromGenericType(typeof(Nullable<>))) return val;

            var prop = val.GetType().GetProperty("Value");
            return prop.GetValue(val, null);
        }
    }
}
