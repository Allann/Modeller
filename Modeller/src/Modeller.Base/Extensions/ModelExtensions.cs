using Hy.Modeller.Models;
using System.Collections.Generic;
using System.Linq;

namespace Hy.Modeller
{
    public static class ModelExtensions
    {
        public static bool HasActive(this Model model) => model.Fields.FirstOrDefault(f => f.Name.Singular.Value == "IsActive") != null;

        public static Field HasBusinessKey(this Model model) => model.Fields.FirstOrDefault(f => f.BusinessKey == true);

        public static bool IsEntity(this Model model) => model?.Key?.Fields !=null && model.Key.Fields.Count == 1 && model.Key.Fields[0].Name.Singular.Value == "Id" && model.Key.Fields[0].DataType == DataTypes.UniqueIdentifier;
    }
}