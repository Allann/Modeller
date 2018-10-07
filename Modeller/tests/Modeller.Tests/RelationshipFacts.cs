using FluentAssertions;
using Modeller.Core;
using Modeller.Models;
using Modeller.Tests.TestJsonFiles;
using System;
using Xunit;

namespace Modeller.Tests
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

        [Fact]
        public static void NullableGuid()
        {
            var f = new Field("Field") { DataType = DataTypes.UniqueIdentifier };

            h.DataType(f).Should().Be("Guid?");
            h.DataType(f, showNullable: false).Should().Be("Guid");
            h.DataType(f, guidNullable: true).Should().Be("Guid?");
        }

        [Fact]
        public static void Guid()
        {
            var f = new Field("Field") { DataType = DataTypes.UniqueIdentifier, Nullable = false };

            h.DataType(f).Should().Be("Guid");
            h.DataType(f, guidNullable: true).Should().Be("Guid?");
        }

        [Fact]
        public static void String()
        {
            var f = new Field("Field") { DataType = DataTypes.String, Nullable = true };

            h.DataType(f).Should().Be("string");
            h.DataType(f, guidNullable: true).Should().Be("string");
        }

        [Fact]
        public static void Object()
        {
            var f = new Field("Field") { DataType = DataTypes.Object, DataTypeTypeName = typeof(Person).Name, Nullable = true };

            h.DataType(f).Should().Be("Person");
            h.DataType(f, guidNullable: true).Should().Be("Person");
        }
    }
}