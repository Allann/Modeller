using FluentAssertions;
using Hy.Modeller.Models;
using Hy.Modeller.Tests.TestJsonFiles;
using Xunit;

namespace Hy.Modeller.Tests
{
    public static class DataTypeFacts
    {
        [Fact]
        public static void RelateEquality()
        {
            var r1 = new Relate(new Name("mod1"), RelationShipTypes.One, new Name("Id"), new Name("mod2"), RelationShipTypes.Many, new Name("mod1Id"));
            var r2 = new Relate(new Name("mod2"), RelationShipTypes.Many, new Name("mod1Id"), new Name("mod1"), RelationShipTypes.One, new Name("Id"));

            r1.Should().Equals(r2);
        }

        [Fact]
        public static void RelateHashCodeEquality()
        {
            var r1 = new Relate(new Name("mod1"), RelationShipTypes.One, new Name("Id"), new Name("mod2"), RelationShipTypes.Many, new Name("mod1Id"));
            var r2 = new Relate(new Name("mod2"), RelationShipTypes.Many, new Name("mod1Id"), new Name("mod1"), RelationShipTypes.One, new Name("Id"));

            r1.GetHashCode().Should().Equals(r2.GetHashCode());
        }

        [Theory]
        [InlineData(DataTypes.Bool, "bool?", "bool", "bool?")]
        [InlineData(DataTypes.Date, "Date?", "Date", "Date?")]
        [InlineData(DataTypes.DateTime, "DateTime?", "DateTime", "DateTime?")]
        [InlineData(DataTypes.Time, "Time?", "Time", "Time?")]
        [InlineData(DataTypes.DateTimeOffset, "DateTimeOffset?", "DateTimeOffset", "DateTimeOffset?")]
        [InlineData(DataTypes.Number, "int?", "int", "int?")]
        [InlineData(DataTypes.UniqueIdentifier, "Guid?", "Guid", "Guid?")]
        [InlineData(DataTypes.Object, "object", "object", "object")]
        [InlineData(DataTypes.String, "string", "string", "string")]
        public static void DateDataTypes(DataTypes dataType, string v1, string v2, string v3)
        {
            var f = new Field("Field") { DataType = dataType };

            f.GetDataType().Should().Be(v1);
            f.GetDataType(showNullable: false).Should().Be(v2);
            f.GetDataType(guidNullable: true).Should().Be(v3);
        }

        [Fact]
        public static void DecimalDataTypes()
        {
            var f = new Field("Field") { DataType = DataTypes.Number, Decimals = 3 };

            f.GetDataType().Should().Be("decimal?");
            f.GetDataType(showNullable: false).Should().Be("decimal");
            f.GetDataType(guidNullable: true).Should().Be("decimal?");
        }

        [Fact]
        public static void DefinedObjectDataTypes()
        {
            var f = new Field("Field") { DataType = DataTypes.Object, DataTypeTypeName = typeof(Person).Name, Nullable = true };

            f.GetDataType().Should().Be("Person");
            f.GetDataType(guidNullable: true).Should().Be("Person");
        }
    }
}