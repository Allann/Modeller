using Hy.Modeller.Models;
using System.Linq;

namespace Hy.Modeller
{
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