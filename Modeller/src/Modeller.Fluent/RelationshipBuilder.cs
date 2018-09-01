using Modeller.Models;
using System;
using System.ComponentModel;

namespace Modeller.Fluent
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RelationshipBuilder : FluentBase
    {
        public RelationshipBuilder(ModuleBuilder module, ModelBuilder model, Models.Relationship relationship)
        {
            ModuleBuild = module ?? throw new ArgumentNullException(nameof(module));
            Build = model ?? throw new ArgumentNullException(nameof(model));
            Instance = relationship;
        }

        public ModuleBuilder ModuleBuild { get; }
        public ModelBuilder Build { get; }
        public Models.Relationship Instance { get; }

        public RelationshipBuilder Relate(string left, string right, RelationShipTypes leftType = RelationShipTypes.One, RelationShipTypes rightType = RelationShipTypes.Many)
        {
            Instance.SetRelationship(left, right, leftType, rightType);
            return this;
        }
    }
}
