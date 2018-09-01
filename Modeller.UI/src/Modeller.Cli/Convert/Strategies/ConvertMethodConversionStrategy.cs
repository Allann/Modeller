using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Convert.Strategies
{
    /// <summary>
    /// Uses a conversion method to convert the value.
    /// </summary>
    public class ConvertMethodConversionStrategy
        : IConvertStrategy
    {

        static ConvertMethodConversionStrategy()
        {
            // Some types have commonly used aliases. These can be used for the ToXXX conversion methods.
            _typeAliases = new Dictionary<Type, string[]>
            {
                { typeof(bool), new string[] { "Bool" } },
                { typeof(int), new string[] { "Integer", "Int" } },
                { typeof(short), new string[] { "Short" } },
                { typeof(ushort), new string[] { "UShort" } },
                { typeof(uint), new string[] { "UInt" } },
                { typeof(long), new string[] { "Long" } },
                { typeof(ulong), new string[] { "ULong" } },
                { typeof(float), new string[] { "Float" } }
            };
        }

        private static Dictionary<Type, string[]> _typeAliases;

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
            var mi = GetInstanceConvertMethod(from, to);
            return mi.Invoke(value, null);
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            return (GetInstanceConvertMethod(from, to) != null);
        }

        private static MethodInfo GetInstanceConvertMethod(Type fromType, Type toType)
        {
            var names = new List<string>
            {
                toType.Name
            };
            if (_typeAliases.ContainsKey(toType))
                names.AddRange(_typeAliases[toType]);

            // Find the convert method.
            // Iterate through each of the alias' to find a method that
            // can be used to convert the value.
            foreach (var name in names)
            {
                var mi = fromType.GetMethod("To" + name, new Type[] { });
                if (mi == null) continue;
                if (mi.ReturnType != toType) continue;
                return mi;
            }

            return null;
        }

    }
}
