using Modeller.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modeller
{
    public static class ModuleExtensions
    {
        public static IEnumerable<Relate> GetRelationships(this Module module, Model model)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var name = model.Name;

            // return the relate struct for ALL relationships that contain the model name within the module
            return module.Models.SelectMany((m, index) => m.Relationships.Where(r => r.LeftModel.Equals(name) || r.RightModel.Equals(name))).Select(re => re.GetDetails(model)).Distinct(new RelateComparer());
        }

        private class RelateComparer : IEqualityComparer<Relate>
        {
            public bool Equals(Relate x, Relate y) => x == null || y == null ? false : x.Equals(y);

            public int GetHashCode(Relate obj) => obj.GetHashCode();
        }
    }

    public static class ModelExtensions
    {
        public static bool HasActive(this Model model) => model.Fields.FirstOrDefault(f => f.Name.Singular.Value == "IsActive") != null;

        public static Field HasBusinessKey(this Model model) => model.Fields.FirstOrDefault(f => f.BusinessKey == true);

        public static bool IsEntity(this Model model) => model.Key.Fields.Count == 1 && model.Key.Fields[0].Name.Singular.Value == "Id" && model.Key.Fields[0].DataType == DataTypes.UniqueIdentifier;

        public static bool IsValid(this Model model)
        {
            //model must have a key
            if (model.Key.Fields.Count == 0)
                return false;
            // model can only have at most, one business key
            if (model.Fields.Count(f => f.BusinessKey == true) > 1)
                return false;

            if (model.HasAudit)
            {
                // ensure fields are not duplicated
                if (model.Fields.Count(f => f.Name.Singular.Value == "Created" || f.Name.Singular.Value == "CreatedBy" || f.Name.Singular.Value == "Modified" || f.Name.Singular.Value == "ModifiedBy") > 0)
                    return false;
            }

            // must have a name
            return string.IsNullOrWhiteSpace(model?.Name?.Singular?.Value) ? false : true;
        }
    }
}