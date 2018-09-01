using System.Linq;
using Modeller.Models;

namespace Modeller
{
    public static class ModelExtensions
    {
        public static bool HasActive(this Model model) => model.Fields.FirstOrDefault(f => f.Name.Singular.Value == "IsActive") != null;

        public static Field HasBusinessKey(this Model model) => model.Fields.FirstOrDefault(f => f.BusinessKey == true);

        public static bool IsEntity(this Model model) => model.Key.Fields.Count == 1 && model.Key.Fields[0].Name.Singular.Value == "Id" && model.Key.Fields[0].DataType == DataTypes.UniqueIdentifier;

        public static bool IsIntersect(this Model model)
        {
            if (model.Key.Fields.Count != 2 || model.Fields.Count != 0 || model.Relationships.Count != 2)
                return false;

            foreach (var item in model.Relationships)
            {
                item.GetMatch(model.Name, out var type, out var field);
                if (type != RelationShipTypes.Many)
                    return false;
            }

            return true;
        }

        public static bool IsValid(this Model model)
        {
            //model must have a key
            if (model.Key.Fields.Count == 0)
                return false;
            // model can only have at most, one business key
            if (model.Fields.Count(f => f.BusinessKey == true) > 1)
                return false;

            if(model.HasAudit)
            {
                // ensure fields are not duplicated
                if (model.Fields.Count(f => f.Name.Singular.Value == "Created" || f.Name.Singular.Value == "CreatedBy" || f.Name.Singular.Value == "Modified" || f.Name.Singular.Value == "ModifiedBy")>0)
                    return false;
            }

            // must have a name
            if (string.IsNullOrWhiteSpace(model?.Name?.Singular?.Value))
                return false;

            return true;
        }
    }
}
