using FluentAssertions;
using Hy.Modeller.Extensions;
using Hy.Modeller.Models;
using Xunit;

namespace Hy.Modeller.Tests
{
    public static class SerialisationFacts
    {
        [Fact]
        public static void CanSerialiseDatabaseModel()
        {
            var expected = new Database
            {
                Name = "databaseName",
                Schema = "schema",
                Server = "server"
            };

            var temp = expected.ToJson();
            var actual = temp.FromJson<Database>();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void CanSerialiseRelationshipModel()
        {
            var expected = new Models.Relationship();
            expected.SetRelationship("left.Id", "right.Id");

            var temp = expected.ToJson();

            var actual = temp.FromJson<Models.Relationship>();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void CanSerialiseDatabaseField()
        {
            var expected = new Models.Field("field")
            {
                BusinessKey = true,
                DataType = Models.DataTypes.Object,
                Decimals = 3,
                DataTypeTypeName = "Test.Types.TypeName",
                Default = "default",
                MaxLength = 34,
                MinLength = 3,
                Nullable = false
            };

            var temp = expected.ToJson();

            var actual = temp.FromJson<Models.Field>();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void CanSerializeModelModel()
        {
            var expected = new Models.Model
            {
                HasAudit = true
            };
            expected.Name.SetName("model");
            expected.Key.Fields.Add(new Models.Field("key") { DataType = Models.DataTypes.UniqueIdentifier, Nullable = false });
            expected.Fields.Add(new Models.Field("Code") { DataType = Models.DataTypes.String, MaxLength = 20, BusinessKey = true, Nullable = false });

            var index = new Models.Index("UX_Code") { IsUnique = true };
            index.Fields.Add(new Models.IndexField("Code") { Sort = System.ComponentModel.ListSortDirection.Descending });
            expected.Indexes.Add(index);

            var r = new Models.Relationship();
            r.SetRelationship("left.Id", "right.Id");
            expected.Relationships.Add(r);

            var temp = expected.ToJson();

            var actual = temp.FromJson<Models.Model>();

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void CanSerialiseDatabaseModule()
        {
            var expected = new Models.Module
            {
                Company = "company"
            };
            expected.Project.SetName("project");
            expected.Feature = new Models.Name("feature");
            var model = new Models.Model
            {
                HasAudit = true
            };
            model.Name.SetName("model");
            model.Key.Fields.Add(new Models.Field("key") { DataType = Models.DataTypes.UniqueIdentifier, Nullable = false });
            model.Fields.Add(new Models.Field("Code") { DataType = Models.DataTypes.String, MaxLength = 20, BusinessKey = true, Nullable = false });

            var index = new Models.Index("UX_Code") { IsUnique = true };
            index.Fields.Add(new Models.IndexField("Code") { Sort = System.ComponentModel.ListSortDirection.Descending });
            model.Indexes.Add(index);

            var r = new Models.Relationship();
            r.SetRelationship("left.Id", "right.Id");
            model.Relationships.Add(r);

            expected.Models.Add(model);

            var temp = expected.ToJson();

            var actual = temp.FromJson<Models.Module>();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}