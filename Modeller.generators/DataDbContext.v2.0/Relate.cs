using Modeller.Models;

namespace DbContext
{
    public struct Relate
    {
        public Relate(Name match, RelationShipTypes matchType, Name matchField, Name other, RelationShipTypes otherType, Name otherField)
        {
            Match = match;
            MatchType = matchType;
            MatchField = matchField;
            Other = other;
            OtherType = otherType;
            OtherField = otherField;
        }

        public Name Match { get; }
        public RelationShipTypes MatchType { get; }
        public Name MatchField { get; }
        public Name Other { get; }
        public RelationShipTypes OtherType { get; }
        public Name OtherField { get; }
    }
}