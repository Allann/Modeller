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

    public static class RelationshipFacts
    {
        [Fact]
        public static void GetOtherReturnsChild()
        {
            var x = new Relationship();
            x.SetRelationship("Parent", "Id", "Child", "ParentId");

            var parent = new Name("Parent");

            x.GetOther(parent, out var type, out var model, out var field);

            type.Should().Be(RelationShipTypes.Many);
            model.Singular.Value.Should().Be("Child");
            field.Singular.Value.Should().Be("ParentId");
        }

        [Fact]
        public static void GetMatchReturnsParent()
        {
            var x = new Relationship();
            x.SetRelationship("Parent", "Id", "Child", "ParentId");

            var parent = new Name("Parent");

            x.GetMatch(parent, out var type, out var field);

            type.Should().Be(RelationShipTypes.One);
            field.Singular.Value.Should().Be("Id");
        }

        [Fact]
        public static void GetOtherThrowsWithInvalidRelationship()
        {
            var x = new Relationship();
            x.SetRelationship("Parent", "Id", "Child", "ParentId");
            var grandParent = new Name("GrandParent");

            var ex = Record.Exception(() => x.GetMatch(grandParent, out var type, out var field));

            ex.Should().BeOfType<ApplicationException>();
        }
    }
}