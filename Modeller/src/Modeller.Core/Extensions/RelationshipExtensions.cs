using Modeller.Models;

namespace Modeller
{
    public static class RelationshipExtensions
    {
        public static RelationShipTypes GetMyRelationshipType(this Relationship relationship, Name name)
        {
            relationship.GetMatch(name, out var type, out var field);
            return type;
        }
        public static RelationShipTypes GetOtherRelationshipType(this Relationship relationship, Name name)
        {
            relationship.GetOther(name, out var type, out var model, out var field);
            return type;
        }

        public static void GetMatch(this Relationship relationship, Name find, out RelationShipTypes type, out Name field)
        {
            if (relationship.RightModel.Equals(find))
            {
                type = relationship.RightType;
                field = relationship.RightField;
            }
            else if (relationship.LeftModel.Equals(find))
            {
                type = relationship.LeftType;
                field = relationship.LeftField;
            }
            else
            {
                throw new System.ApplicationException($"Relationship not found {find}");
            }
        }

        public static void GetOther(this Relationship relationship, Name find, out RelationShipTypes type, out Name model, out Name field)
        {
            if (relationship.LeftModel.Equals(find))
            {
                type = relationship.RightType;
                model = relationship.RightModel;
                field = relationship.RightField;
            }
            else if (relationship.RightModel.Equals(find))
            {
                type = relationship.LeftType;
                model = relationship.LeftModel;
                field = relationship.LeftField;
            }
            else
            {
                throw new System.ApplicationException("Relationship not found");
            }
        }


        public static Relate GetDetails(this Relationship relationship, Name match)
        {
            relationship.GetMatch(match, out var matchType, out var matchField);
            relationship.GetOther(match, out var otherType, out var otherModel, out var otherField);

            return new Relate(match, matchType, matchField, otherModel, otherType, otherField);
        }
    }
}
