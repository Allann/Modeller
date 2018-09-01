using Modeller.Models;

namespace Modeller.UI.Wpf
{
    public class Index : SimpleObject
    {
        private string _leftTabel;
        private string _rightColumn;
        private string _rightTable;
        private string _leftColumn;
        private RelationShipTypes _leftType;
        private RelationShipTypes _rightType;

        public Index(Table table)
        {
            Parent = table;
        }

        public string LeftTable
        {
            get => _leftTabel;
            set
            {
                _leftTabel = value;
                OnPropertyChanged(nameof(LeftTable));
            }
        }

        public string LeftColumn
        {
            get => _leftColumn;
            set
            {
                _leftColumn = value;
                OnPropertyChanged(nameof(LeftColumn));
            }
        }

        public string RightTable
        {
            get => _rightTable;
            set
            {
                _rightTable = value;
                OnPropertyChanged(nameof(RightTable));
            }
        }

        public string RightColumn
        {
            get => _rightColumn;
            set
            {
                _rightColumn = value;
                OnPropertyChanged(nameof(RightColumn));
            }
        }

        public RelationShipTypes LeftType { get => _leftType; set { _leftType = value; OnPropertyChanged(nameof(LeftType)); } }
        public RelationShipTypes RightType { get => _rightType; set { _rightType = value; OnPropertyChanged(nameof(RightType)); } }
        public Table Parent { get; }
    }
}
