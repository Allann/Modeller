using System;

namespace Modeller.Models
{
    public class Relationship
    {
        public void SetRelationship(string left, string right, RelationShipTypes leftType = RelationShipTypes.One, RelationShipTypes rightType = RelationShipTypes.Many)
        {
            if (string.IsNullOrWhiteSpace(left))
            {
                throw new System.ArgumentException("Must include a Left field in the format model.field", nameof(left));
            }
            if (string.IsNullOrWhiteSpace(right))
            {
                throw new System.ArgumentException("Must include a Right field in the format model.field", nameof(right));
            }
            var lefts = left.Split('.');
            if (lefts.Length != 2)
            {
                throw new System.ArgumentException("Must include a Left field in the format model.field", nameof(left));
            }

            var rights = right.Split('.');
            if (rights.Length != 2)
            {
                throw new System.ArgumentException("Must include a Left field in the format model.field", nameof(right));
            }

            LeftModel = new Name(lefts[0]);
            LeftField = new Name(lefts[1]);
            LeftType = leftType;
            RightModel = new Name(rights[0]);
            RightField = new Name(rights[1]);
            RightType = rightType;
        }

        public void SetRelationship(string leftModel, string leftField, string rightModel, string rightField, RelationShipTypes leftType = RelationShipTypes.One, RelationShipTypes rightType = RelationShipTypes.Many)
        {
            if (string.IsNullOrWhiteSpace(leftModel))
            {
                throw new System.ArgumentException(nameof(leftModel));
            }

            if (string.IsNullOrWhiteSpace(leftField))
            {
                throw new System.ArgumentException(nameof(leftField));
            }

            if (string.IsNullOrWhiteSpace(rightModel))
            {
                throw new System.ArgumentException(nameof(rightModel));
            }

            if (string.IsNullOrWhiteSpace(rightField))
            {
                throw new System.ArgumentException(nameof(rightField));
            }

            LeftModel = new Name(leftModel);
            LeftField = new Name(leftField);
            LeftType = leftType;
            RightModel = new Name(rightModel);
            RightField = new Name(rightField);
            RightType = rightType;
        }

        public Name LeftModel { get; set; } = new Name("");

        public Name LeftField { get; set; } = new Name("");

        public RelationShipTypes LeftType { get; set; }

        public Name RightModel { get; set; } = new Name("");

        public Name RightField { get; set; } = new Name("");

        public RelationShipTypes RightType { get; set; }

        public void GetMatch(Name find, out RelationShipTypes type, out Name field)
        {
            if (RightModel.Equals(find))
            {
                type = RightType;
                field = RightField;
            }
            else if (LeftModel.Equals(find))
            {
                type = LeftType;
                field = LeftField;
            }
            else
            {
                throw new ApplicationException("Relationship not found");
            }
        }

        public void GetOther(Name find, out RelationShipTypes type, out Name model, out Name field)
        {
            if (LeftModel.Equals(find))
            {
                type = RightType;
                model = RightModel;
                field = RightField;
            }
            else if (RightModel.Equals(find))
            {
                type = LeftType;
                model = LeftModel;
                field = LeftField;
            }
            else
            {
                throw new ApplicationException("Relationship not found");
            }
        }
    }
}