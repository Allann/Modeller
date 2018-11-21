using FluentAssertions;
using Hy.Modeller.Extensions;
using System.Linq;
using Xunit;

namespace Hy.Modeller.Tests
{
    public static class ModuleFacts
    {
        private static string _testModule = $"{{\"company\":\"{Defaults.CompanyName}\",\"project\":\"\",\"models\":[]}}";

        [Fact]
        public static void CanSerialiseToJson()
        {
            var x = new Models.Module();

            var json = x.ToJson();

            json.Should().Be(_testModule);
        }

        [Fact]
        public static void CanDeserialiseFromJson()
        {
            var obj = _testModule.FromJson<Models.Module>();

            obj.Should().BeOfType<Models.Module>().And.NotBeNull();
        }

        [Fact]
        public static void ExtensionReturnsDistinctRelationshipForAModel()
        {
            var module = Fluent.Module.Create("module")
                .AddModel("mod1")
                    .WithDefaultKey()
                    .AddRelationship().Relate("mod1.Id", "mod2.mod1Id").Build
                    .Build
                .AddModel("mod2")
                    .WithDefaultKey()
                    .AddField("mod1Id").DataType(Models.DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddRelationship().Relate("mod2.mod1Id", "mod1.Id", Models.RelationShipTypes.Many, Models.RelationShipTypes.One).Build
                    .AddRelationship().Relate("mod2.Id", "mod3.Id", Models.RelationShipTypes.Many, Models.RelationShipTypes.Many).Build
                    .Build
                .AddModel("mod3")
                    .WithDefaultKey()
                    .Build
                .AddModel("mod4")
                    .WithDefaultKey()
                    .AddField("mod1Id").DataType(Models.DataTypes.UniqueIdentifier).Nullable(false).Build
                    .AddRelationship().Relate("mod1.Id", "mod4.mod1Id", Models.RelationShipTypes.One, Models.RelationShipTypes.One).Build
                    .Build
                .Build;

            var mod1 = module.Models.Single(m => m.Name.Singular.Value == "Mod1");
            var mod2 = module.Models.Single(m => m.Name.Singular.Value == "Mod2");
            var mod3 = module.Models.Single(m => m.Name.Singular.Value == "Mod3");
            var mod4 = module.Models.Single(m => m.Name.Singular.Value == "Mod4");

            var actual = module.GetRelationships(mod1);
            actual.Should().HaveCount(2);
            actual = module.GetRelationships(mod2);
            actual.Should().HaveCount(2);
            actual = module.GetRelationships(mod3);
            actual.Should().HaveCount(1);
            actual = module.GetRelationships(mod4);
            actual.Should().HaveCount(1);
        }

        [Fact]
        public static void ManyToManySetsDefinedIn()
        {
            var module = Fluent.Module.Create("module")
                .AddModel("mod1")
                    .WithDefaultKey()
                    .AddRelationship().Relate("mod1.Id", "mod2.Id", Models.RelationShipTypes.Many, Models.RelationShipTypes.Many).Build
                    .Build
                .AddModel("mod2")
                    .WithDefaultKey()
                    .Build
                .Build;

            var mod1 = module.Models.Single(m => m.Name.Singular.Value == "Mod1");
            var mod2 = module.Models.Single(m => m.Name.Singular.Value == "Mod2");

            var mod1r = module.GetRelationships(mod1);
            mod1r.Should().HaveCount(1);
            mod1r.First().DefinedIn.Singular.Value.Should().Equals("Mod1");

            var mod2r = module.GetRelationships(mod2);
            mod2r.Should().HaveCount(1);
            mod2r.First().DefinedIn.Singular.Value.Should().Equals("Mod1");
        }
    }
}