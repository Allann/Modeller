using Hy.Modeller.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hy.Modeller
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
}