using Modeller.Models;

namespace Modeller
{
    public static class RelationshipExtensions
    {
        public static Relate GetDetails(this Relationship relationship, Model find)
        {
            RelationShipTypes matchType;
            RelationShipTypes otherType;
            Name matchName;
            Name otherName;
            Name matchField;
            Name otherField;

            if (relationship.RightModel.Equals(find.Name))
            {
                matchName = relationship.RightModel;
                matchType = relationship.RightType;
                matchField = relationship.RightField;
                otherName = relationship.LeftModel;
                otherType = relationship.LeftType;
                otherField = relationship.LeftField;
            }
            else if (relationship.LeftModel.Equals(find.Name))
            {
                otherName = relationship.RightModel;
                otherType = relationship.RightType;
                otherField = relationship.RightField;
                matchName = relationship.LeftModel;
                matchType = relationship.LeftType;
                matchField = relationship.LeftField;
            }
            else
            {
                throw new System.ApplicationException($"Relationship not found {find}");
            }

            if (relationship.LeftType == RelationShipTypes.Many && relationship.RightType == RelationShipTypes.Many)
            {
                return find.Relationships.Contains(relationship)
                    ? new Relate(matchName, matchType, matchField, otherName, otherType, otherField, matchName)
                    : new Relate(matchName, matchType, matchField, otherName, otherType, otherField, otherName);
            }
            return new Relate(matchName, matchType, matchField, otherName, otherType, otherField);
        }
    }
}
