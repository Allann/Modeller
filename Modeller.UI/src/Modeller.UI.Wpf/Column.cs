using Modeller.Models;

namespace Modeller.UI.Wpf
{
    public class Column : SimpleObject
    {
        private string _name;
        private string _image;
        private bool _isBusinessKey;
        private DataTypes _dataType;
        private int? _decimals;
        private string _default;
        private int? _maxLength;
        private int? _minLength;
        private bool _isNullable;

        public Column(Table table)
        {
            Parent = table;
        }

        public Table Parent { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Image
        {
            get => _image ?? (IsNullable ? "images\\application-small.png" : "images\\application-small-blue.png");
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        public bool IsBusinessKey { get => _isBusinessKey; set { _isBusinessKey = value; OnPropertyChanged(nameof(IsBusinessKey)); } }
        public DataTypes DataType { get => _dataType; set { _dataType = value; OnPropertyChanged(nameof(DataType)); } }
        public int? Decimals { get => _decimals; set { _decimals = value; OnPropertyChanged(nameof(Decimals)); } }
        public string Default { get => _default; set { _default = value; OnPropertyChanged(nameof(Default)); } }
        public int? MaxLength { get => _maxLength; set { _maxLength = value; OnPropertyChanged(nameof(MaxLength)); } }
        public int? MinLength { get => _minLength; set { _minLength = value; OnPropertyChanged(nameof(MinLength)); } }
        public bool IsNullable { get => _isNullable; set { _isNullable = value; OnPropertyChanged(nameof(IsNullable)); } }

        public string Details
        {
            get
            {
                var result = DataType.ToString();
                if (DataType == DataTypes.String)
                {
                    if (MaxLength.HasValue)
                        result += $" ({MaxLength.Value})";
                }
                return result;
            }
        }
    }
}
