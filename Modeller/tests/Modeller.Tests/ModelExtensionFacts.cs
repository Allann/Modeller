using FluentAssertions;
using Hy.Modeller.Models;
using Xunit;

namespace Hy.Modeller.Tests
{
    public class ModelExtensionFacts
    {
        [Theory]
        [InlineData("IsActive", true)]
        [InlineData("isactive", false)]
        [InlineData("active", false)]
        [InlineData("enable", false)]
        public void ModelHasActive(string name, bool result)
        {
            var model = new Model();
            model.Fields.Add(new Field(name));
            model.HasActive().Should().Be(result);
        }

        [Fact]
        public void ModelWithNoFieldsDoesntFail()
        {
            var model = new Model();
            model.HasActive().Should().Be(false);
        }

        [Fact]
        public void ModelHasBusinessKey()
        {
            var model = new Model();
            var field = new Field("Code") { BusinessKey = true };
            model.Fields.Add(field);
            model.HasBusinessKey().Should().Be(field);
        }

        [Fact]
        public void ModelDoesntHaveBusinessKey()
        {
            var model = new Model();
            var field = new Field("Code");
            model.Fields.Add(field);
            model.HasBusinessKey().Should().BeNull();
        }

        [Fact]
        public void ModelIsEntity()
        {
            var model = new Model();
            var field = new Field("Id") { DataType=DataTypes.UniqueIdentifier };
            model.Key.Fields.Add(field);
            model.IsEntity().Should().Be(true);
        }

        [Theory]
        [InlineData("Id", DataTypes.Number)]
        [InlineData("ModelId", DataTypes.UniqueIdentifier)]
        public void ModelIsNotEntity(string name, DataTypes dataType)
        {
            var model = new Model();
            var field = new Field(name) { DataType = dataType };
            model.Key.Fields.Add(field);
            model.IsEntity().Should().Be(false);
        }

        [Fact]
        public void ModelIsNotEntityWhenCompositeKey()
        {
            var model = new Model();
            var field1 = new Field("Id") { DataType = DataTypes.UniqueIdentifier };
            var field2 = new Field("AnotherId") { DataType = DataTypes.UniqueIdentifier };
            model.Key.Fields.Add(field1);
            model.Key.Fields.Add(field2);
            model.IsEntity().Should().Be(false);
        }

        [Fact]
        public void ModelIsValid()
        {
            var model = new Model() { Name=new Name("Test") };
            var id = new Field("Id") { DataType = DataTypes.UniqueIdentifier };
            model.Key.Fields.Add(id);
            var field = new Field("Code");
            model.Fields.Add(field);
            model.IsValid().Should().Be(true);
            model.Errors.Should().BeEmpty();
        }

        [Fact]
        public void ModelIsNotValidWithNoKeyFields()
        {
            var model = new Model() { Name = new Name("Test") };
            var field = new Field("Code");
            model.Fields.Add(field);
            model.IsValid().Should().Be(false);
            model.Errors.Should().BeEquivalentTo("Model Test must have a key");
        }

        [Fact]
        public void ModelIsNotValidWithMultipleBusinessKeys()
        {
            var model = new Model() { Name = new Name("Test") };
            var id = new Field("Id") { DataType = DataTypes.UniqueIdentifier };
            model.Key.Fields.Add(id);
            model.Fields.Add(new Field("Code") { BusinessKey = true });
            model.Fields.Add(new Field("Name") { BusinessKey = true });
            model.IsValid().Should().Be(false);
            model.Errors.Should().BeEquivalentTo("Model Test can only have at most, one business key");
        }

        [Fact]
        public void ModelIsNotValidIfAuditFieldsAreDupllicated()
        {
            var model = new Model() { Name = new Name("Test"), HasAudit=true };
            model.Key.Fields.Add(new Field("Id") { DataType = DataTypes.UniqueIdentifier });
            model.Fields.Add(new Field("Code") { BusinessKey = true });
            model.IsValid().Should().Be(true);
            model.Fields.Add(new Field("CreatedBy"));
            model.IsValid().Should().Be(false);
            model.Errors.Should().BeEquivalentTo("Model Test Audit fields shouldn't be added when HasAudit is true");
        }

        [Fact]
        public void ModelIsNotValidWithDuplicateFields()
        {
            var model = new Model() { Name = new Name("Test") };
            var id = new Field("Id") { DataType = DataTypes.UniqueIdentifier };
            model.Key.Fields.Add(id);
            model.Fields.Add(new Field("Code"));
            model.Fields.Add(new Field("Code"));
            model.IsValid().Should().Be(false);
            model.Errors.Should().BeEquivalentTo("Model Test shouldn't have duplicate field names (Code)");
        }

        [Fact]
        public void ModelIsNotValidIfAnyFieldsHaveNegativeDecimals()
        {
            var model = new Model() { Name = new Name("Test") };
            var id = new Field("Id") { DataType = DataTypes.UniqueIdentifier };
            model.Key.Fields.Add(id);
            model.Fields.Add(new Field("Code") { Decimals = -2 });
            model.IsValid().Should().Be(false);
            model.Errors.Should().BeEquivalentTo("Field Code.Decimals must be zero or greater");
        }
    }
}