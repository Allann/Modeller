using Hy.Modeller.Models;

namespace Hy.Modeller
{
    public static class FieldExtensions
    {
        public static string GetDataType(this Field field, bool showNullable = true, bool guidNullable = false)
        {
            string type;
            switch (field.DataType)
            {
                case DataTypes.Bool:
                    type = "bool";
                    break;

                case DataTypes.Date:
                case DataTypes.DateTime:
                case DataTypes.Time:
                case DataTypes.DateTimeOffset:
                    type = field.DataType.ToString();
                    break;

                case DataTypes.Number:
                    type = field.Decimals.HasValue && field.Decimals.Value > 0 ? "decimal" : "int";
                    break;

                case DataTypes.UniqueIdentifier:
                    type = "Guid";
                    break;

                case DataTypes.Object:
                    type = string.IsNullOrWhiteSpace(field.DataTypeTypeName) ? "object" : field.DataTypeTypeName;
                    showNullable = false;
                    break;

                default:
                    type = "string";
                    showNullable = false;
                    break;
            }
            if (showNullable && (field.Nullable || (type == "Guid" && guidNullable)))
                type += "?";
            return type;
        }
    }
}