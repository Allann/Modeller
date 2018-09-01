using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Core.Extensions
{
    public static class CoreExtensions
    {

        /// <summary>
        /// Uses DataAnnotations to validate the properties of the object.
        /// </summary>
        /// <param name="obj"></param>
        public static ValidationResult[] Validate(this object obj)
        {
            var ctx = new ValidationContext(obj, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(obj, ctx, results, true);
            return results.ToArray();
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static Array Shrink(this Array arr, int startIndex, int endIndex)
        {
            if (arr == null)
                return null;
            if (startIndex >= arr.Length)
                return Array.CreateInstance(arr.GetType().GetElementType(), 0);
            if (endIndex < startIndex)
                return Array.CreateInstance(arr.GetType().GetElementType(), 0);
            if (startIndex < 0)
                startIndex = 0;

            var length = (endIndex - startIndex) + 1;
            var retArr = Array.CreateInstance(arr.GetType().GetElementType(), length);
            for (var i = startIndex; i <= endIndex; i++)
                retArr.SetValue(arr.GetValue(i), i - startIndex);

            return retArr;
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string[] Shrink(this string[] arr, int startIndex)
        {
            return Shrink((Array)arr, startIndex, arr.Length - 1) as string[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string[] Shrink(this string[] arr, int startIndex, int endIndex)
        {
            return Shrink((Array)arr, startIndex, endIndex) as string[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int[] Shrink(this int[] arr, int startIndex)
        {
            return Shrink((Array)arr, startIndex, arr.Length - 1) as int[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static int[] Shrink(this int[] arr, int startIndex, int endIndex)
        {
            return Shrink((Array)arr, startIndex, endIndex) as int[];
        }

        /// <summary>
        /// Gets a value that determines if the type allows instances with a null value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return type == null ? false : !type.IsValueType ? true : type.IsDerivedFromGenericType(typeof(Nullable<>));
        }

        /// <summary>
        /// Determines if the type corresponds to one of the built in numeric types (such as int, double, etc).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            type = GetTrueType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the C# name of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCSharpName(this Type type)
        {
            var name = type.Name;

            if (type == typeof(bool))
                name = "bool";
            else if (type == typeof(byte))
                name = "byte";
            else if (type == typeof(sbyte))
                name = "sbyte";
            else if (type == typeof(char))
                name = "char";
            else if (type == typeof(short))
                name = "short";
            else if (type == typeof(ushort))
                name = "ushort";
            else if (type == typeof(int))
                name = "int";
            else if (type == typeof(uint))
                name = "uint";
            else if (type == typeof(long))
                name = "long";
            else if (type == typeof(ulong))
                name = "ulong";
            else if (type == typeof(float))
                name = "float";
            else if (type == typeof(double))
                name = "double";
            else if (type == typeof(decimal))
                name = "decimal";
            else if (type == typeof(string))
                name = "string";

            if (type.IsValueType && type.IsNullable())
                name += "?";

            return name;
        }

        /// <summary>
        /// Converts the array to a different type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] Convert<T>(this Array arr)
        {
            return (T[])Convert(arr, typeof(T));
        }

        /// <summary>
        /// Converts the array to a different type.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static Array Convert(this Array arr, Type elementType)
        {
            if (arr.GetType().GetElementType() == elementType)
                return arr.Copy();

            var retArr = Array.CreateInstance(elementType, arr.Length);
            for (var i = 0; i < arr.Length; i++)
                retArr.SetValue(ConvertEx.ChangeType(arr.GetValue(i), elementType), i);
            return retArr;
        }

        /// <summary>
        /// Copies the array to a new array of the same type.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Array Copy(this Array arr)
        {
            var newArr = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
            arr.CopyTo(newArr, 0);
            return newArr;
        }

        /// <summary>
        /// Gets the underlying type if the type is Nullable, otherwise just returns the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTrueType(this Type type)
        {
            // check for Nullable enums. 
            // Null values should be handled by DefaultValueConversionStrategy, but we need to be able
            // to get the actual type of the enum here.
            return IsDerivedFromGenericType(type, typeof(Nullable<>)) ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Determines if the type is an instance of a generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsDerivedFromGenericType(this Type type, Type genericType)
        {
            var typeTmp = type;
            while (typeTmp != null)
            {
                if (typeTmp.IsGenericType && typeTmp.GetGenericTypeDefinition() == genericType)
                    return true;

                typeTmp = typeTmp.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Determines if the type is derived from the given base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsDerivedFrom(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }
        
        /// <summary>
        /// Determines if the type implements the given interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool Implements(this Type type, Type interfaceType)
        {
            foreach (var i in type.GetInterfaces())
                if (i == interfaceType)
                    return true;
            return false;
        }

        /// <summary>
        /// Gets the specified attribute from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyDescriptor prop) where T : Attribute
        {
            foreach (Attribute att in prop.Attributes)
            {
                if (att is T tAtt)
                    return tAtt;
            }
            return null;
        }

        /// <summary>
        /// Gets the specified attributes from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this PropertyDescriptor prop) where T : Attribute
        {
            var atts = new List<T>();
            foreach (Attribute att in prop.Attributes)
            {
                if (att is T tAtt)
                    atts.Add(tAtt);
            }
            return atts.ToArray();
        }

        /// <summary>
        /// Gets the specified attribute from the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var atts = type.GetCustomAttributes(typeof(T), inherit);
            return atts.Length == 0 ? null : atts[0] as T;
        }

        /// <summary>
        /// Gets the specified attribute from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj, bool inherit) where T : Attribute
        {
            if (obj == null)
                return null;

            var type = obj.GetType();
            return type.IsDerivedFrom(typeof(PropertyDescriptor))
                ? GetAttribute<T>((PropertyDescriptor)obj)
                : type.IsDerivedFrom(typeof(PropertyInfo))
                ? GetAttribute<T>((PropertyInfo)obj, inherit)
                : type.IsDerivedFrom(typeof(Assembly))
                ? GetAttribute<T>((Assembly)obj, inherit)
                : type.IsDerivedFrom(typeof(Type)) ? GetAttribute<T>((Type)obj, inherit) : GetAttribute<T>(type, inherit);
        }

        /// <summary>
        /// Gets the specified attribute for the assembly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Assembly asm) where T : Attribute
        {
            if (asm == null)
                return null;

            var atts = asm.GetCustomAttributes(typeof(T), false);
            return atts == null ? null : atts.Length == 0 ? null : (T)atts[0];
        }

        /// <summary>
        /// Gets the details of an exception suitable for display.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetDetails(this Exception ex)
        {
            var details = new StringBuilder();

            while (ex != null)
            {
                details.AppendLine(ex.GetType().FullName);
                details.AppendLine(ex.Message);
                details.AppendLine(ex.StackTrace);

                ex = ex.InnerException;
                if (ex != null)
                {
                    details.AppendLine();
                    details.AppendLine(new string('#', 70));
                }
            }

            return details.ToString();
        }
    }
}
