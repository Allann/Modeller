using System.Collections.ObjectModel;

namespace Modeller.UI.Wpf
{
    public class Table : SimpleObject
    {
        private string _name;
        private bool _isEntity;
        private bool _isAuditable;
        private bool _supportActive;
        private bool _supportSoftDelete;

        public Table(Database parent)
        {
            Parent = parent;
        }

        public Database Parent { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool IsEntity
        {
            get => _isEntity;
            set
            {
                _isEntity = value;
                OnPropertyChanged(nameof(IsEntity));
            }
        }

        public bool IsAuditable
        {
            get => _isAuditable; set { _isAuditable = value; OnPropertyChanged(nameof(IsAuditable)); }
        }

        public bool SupportActive { get => _supportActive; set { _supportActive = value; OnPropertyChanged(nameof(SupportActive)); } }

        public bool SupportSoftDelete { get => _supportSoftDelete; set { _supportSoftDelete = value; OnPropertyChanged(nameof(SupportSoftDelete)); } }

        public ObservableCollection<Column> Key { get; } = new ObservableCollection<Column>();

        public ObservableCollection<Column> Columns { get; } = new ObservableCollection<Column>();

        public ObservableCollection<Index> Relationships { get; } = new ObservableCollection<Index>();

    }
}
