using FluentAssertions;
using Modeller.Extensions;
using Xunit;

namespace Modeller.Tests
{
    public static class ModelFacts
    {
        private const string _testModel = @"{""name"":"""",""key"":{""fields"":[]},""fields"":[],""relationships"":[]}";

        [Fact]
        public static void CanSerialiseToJson()
        {
            var x = new Models.Model();

            var json = x.ToJson();

            json.Should().Be(_testModel);
        }

        [Fact]
        public static void CanDeserialiseFromJson()
        {
            var obj = _testModel.FromJson<Models.Model>();

            obj.Should().BeOfType<Models.Model>().And.NotBeNull();
        }

        [Fact]
        public static void IsNotIntersectModelHasField()
        {
            var module = Fluent.Module
                .Create("Test")
                .AddModel("Intersect")
                    .WithKey()
                        .AddField("Table1Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .AddField("Table2Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .Build
                    .AddField("Field1").Build
                    .AddRelationship().Relate("Table1.Id", "Intersect.Table1Id").Build
                    .AddRelationship().Relate("Table2.Id", "Intersect.Table2Id").Build
                    .Build
                .Build;

            var model = module.Models[0];

            model.Should().BeOfType<Models.Model>().And.NotBeNull();
            model.IsIntersect().Should().BeFalse();
        }

        [Fact]
        public static void IsNotIntersectModelMissingRelationship()
        {
            var module = Fluent.Module
                .Create("Test")
                .AddModel("Intersect")
                    .WithKey()
                        .AddField("Table1Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .AddField("Table2Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .Build
                    .AddRelationship().Relate("Table1.Id", "Intersect.Table1Id").Build
                    .Build
                .Build;

            var model = module.Models[0];

            model.Should().BeOfType<Models.Model>().And.NotBeNull();
            model.IsIntersect().Should().BeFalse();
        }

        [Fact]
        public static void IsNotIntersectModelMissingKey()
        {
            var module = Fluent.Module
                .Create("Test")
                .AddModel("Intersect")
                    .WithKey()
                        .AddField("Table1Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .Build
                    .AddRelationship().Relate("Table1.Id", "Intersect.Table1Id").Build
                    .AddRelationship().Relate("Table2.Id", "Intersect.Table2Id").Build
                    .Build
                .Build;

            var model = module.Models[0];

            model.Should().BeOfType<Models.Model>().And.NotBeNull();
            model.IsIntersect().Should().BeFalse();
        }

        [Fact]
        public static void IsIntersectModel()
        {
            var module = Fluent.Module
                .Create("Test")
                .AddModel("Intersect")
                    .WithKey()
                        .AddField("Table1Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .AddField("Table2Id").DataType(Models.DataTypes.UniqueIdentifier).Build
                        .Build
                    .AddRelationship().Relate("Table1.Id", "Intersect.Table1Id").Build
                    .AddRelationship().Relate("Table2.Id", "Intersect.Table2Id").Build
                    .Build
                .Build;

            var model = module.Models[0];

            model.Should().BeOfType<Models.Model>().And.NotBeNull();
            model.IsIntersect().Should().BeTrue();
        }
    }
}