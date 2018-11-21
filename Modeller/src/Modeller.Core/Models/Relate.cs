using System;
using System.Collections.Generic;

namespace Hy.Modeller.Models
{
    public struct Relate : IEquatable<Relate>
    {
        public Relate(Name match, RelationShipTypes matchType, Name matchField, Name other, RelationShipTypes otherType, Name otherField, Name definedIn = null)
        {
            Match = match;
            MatchType = matchType;
            MatchField = matchField;
            Other = other;
            OtherType = otherType;
            OtherField = otherField;
            DefinedIn = definedIn;
        }

        public Name DefinedIn { get; set; }

        public Name Match { get; }
        public RelationShipTypes MatchType { get; }
        public Name MatchField { get; }
        public Name Other { get; }
        public RelationShipTypes OtherType { get; }
        public Name OtherField { get; }

        public override bool Equals(object obj) => obj is Relate && Equals((Relate)obj);

        public bool Equals(Relate other) => EqualityComparer<Name>.Default.Equals(Match, other.Match) && MatchType == other.MatchType && EqualityComparer<Name>.Default.Equals(MatchField, other.MatchField) && EqualityComparer<Name>.Default.Equals(Other, other.Other) && OtherType == other.OtherType && EqualityComparer<Name>.Default.Equals(OtherField, other.OtherField);

        public override int GetHashCode()
        {
            var hashCode = -644701969;
            hashCode = hashCode * -1521134295 + EqualityComparer<Name>.Default.GetHashCode(Match);
            hashCode = hashCode * -1521134295 + MatchType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Name>.Default.GetHashCode(MatchField);
            hashCode = hashCode * -1521134295 + EqualityComparer<Name>.Default.GetHashCode(Other);
            hashCode = hashCode * -1521134295 + OtherType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Name>.Default.GetHashCode(OtherField);
            return hashCode;
        }

        public static bool operator ==(Relate relate1, Relate relate2) => relate1.Equals(relate2);

        public static bool operator !=(Relate relate1, Relate relate2) => !(relate1 == relate2);
    }
}