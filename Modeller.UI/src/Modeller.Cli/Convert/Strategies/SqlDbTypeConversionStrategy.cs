using System;
using System.Collections.Generic;
using System.Data;
using Core.Extensions;

namespace Core.Convert.Strategies
{

    /// <summary>
    /// Converts a .Net type to a SqlDBType.
    /// </summary>
    public class SqlDBTypeConversionStrategy
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
            if (from == typeof(SqlDbType))
                return DbTypeMap.ToNetType((SqlDbType)value);
            else
            {
                var type = (Type)value;
                // check for Nullable enums. 
                // Null values should be handled by DefaultValueConversionStrategy, but we need to be able
                // to get the actual type of the enum here.
                if (type.IsDerivedFromGenericType(typeof(Nullable<>)))
                    type = type.GetGenericArguments()[0];
                return DbTypeMap.ToSqlDbType(type);
            }
        }

        /// <summary>
        /// Determines whether this converter can convert the value.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool CanConvert(Type from, Type to)
        {
            return from.IsDerivedFrom(typeof(Type)) && to == typeof(SqlDbType)
                ? true
                : to.IsDerivedFrom(typeof(Type)) && from == typeof(SqlDbType) ? true : false;
        }

    }

    /// <summary>
    /// Map between different datatypes.
    /// </summary>
    public static class DbTypeMap
    {
        static DbTypeMap()
        {
            _map = new List<DbTypeMapEntry>
            {
                new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit),
                new DbTypeMapEntry(typeof(byte), DbType.Byte, SqlDbType.TinyInt),
                new DbTypeMapEntry(typeof(byte?), DbType.Byte, SqlDbType.TinyInt),
                new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Binary),
                new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime),
                new DbTypeMapEntry(typeof(DateTime?), DbType.DateTime, SqlDbType.DateTime),
                new DbTypeMapEntry(typeof(decimal), DbType.Decimal, SqlDbType.Decimal),
                new DbTypeMapEntry(typeof(decimal?), DbType.Decimal, SqlDbType.Decimal),
                new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float),
                new DbTypeMapEntry(typeof(double?), DbType.Double, SqlDbType.Float),
                new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier),
                new DbTypeMapEntry(typeof(Guid?), DbType.Guid, SqlDbType.UniqueIdentifier),
                new DbTypeMapEntry(typeof(short), DbType.Int16, SqlDbType.SmallInt),
                new DbTypeMapEntry(typeof(short?), DbType.Int16, SqlDbType.SmallInt),
                new DbTypeMapEntry(typeof(int), DbType.Int32, SqlDbType.Int),
                new DbTypeMapEntry(typeof(int?), DbType.Int32, SqlDbType.Int),
                new DbTypeMapEntry(typeof(long), DbType.Int64, SqlDbType.BigInt),
                new DbTypeMapEntry(typeof(long?), DbType.Int64, SqlDbType.BigInt),
                new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant),
                new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar)
            };
        }

        private static List<DbTypeMapEntry> _map;

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ToNetType(SqlDbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("SqlDbType.{0} cannot be converted to a .Net type.", type.ToString()));
            return entry.Type;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ToNetType(DbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("DbType.{0} cannot be converted to a .Net type.", type.ToString()));
            return entry.Type;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(SqlDbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a DbType.", type.ToString()));
            return entry.DbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(Type type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a DbType.", type.ToString()));
            return entry.DbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(DbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a SqlDbType.", type.ToString()));
            return entry.SqlDbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(Type type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a SqlDbType.", type.ToString()));
            return entry.SqlDbType;
        }

        private static DbTypeMapEntry Find(Type type)
        {
            return _map.Find((e) => { return e.Type == type; });
        }

        private static DbTypeMapEntry Find(DbType type)
        {
            return _map.Find((e) => { return e.DbType == type; });
        }

        private static DbTypeMapEntry Find(SqlDbType type)
        {
            return _map.Find((e) => { return e.SqlDbType == type; });
        }

        /// <summary>
        /// Determines if the .Net type can be converted to a SqlDbType/DbType or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanConvertType(Type type)
        {
            var entry = Find(type);
            return entry != null;
        }

        /// <summary>
        /// Represents a map entry for conversion.
        /// </summary>
        private class DbTypeMapEntry
        {

            /// <summary>
            /// Creates an instance of DbTypeMapEntry.
            /// </summary>
            /// <param name="type"></param>
            /// <param name="dbType"></param>
            /// <param name="sqlDbType"></param>
            public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
            {
                Type = type;
                DbType = dbType;
                SqlDbType = sqlDbType;
            }

            /// <summary>
            /// Gets the .Net type.
            /// </summary>
            public Type Type { get; private set; }

            /// <summary>
            /// Gets the DbType.
            /// </summary>
            public DbType DbType { get; private set; }

            /// <summary>
            /// Gets the SqlDbType.
            /// </summary>
            public SqlDbType SqlDbType { get; private set; }

        }

    }

}
