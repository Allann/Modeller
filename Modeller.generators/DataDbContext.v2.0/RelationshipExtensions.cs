using Modeller.Models;

namespace DbContext
{
    internal static class RelationshipExtensions
    {
        public static Relate GetDetails(this Relationship relationship, Name match)
        {
            relationship.GetMatch(match, out var matchType, out var matchField);
            relationship.GetOther(match, out var otherType, out var otherModel, out var otherField);

            return new Relate(match, matchType, matchField, otherModel, otherType, otherField);
        }
    }
}