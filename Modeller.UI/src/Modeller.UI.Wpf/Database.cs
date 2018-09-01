using System.Collections.ObjectModel;

namespace Modeller.UI.Wpf
{
    public class Database : SimpleObject
    {
        private string _company;
        private string _feature;
        private string _project;

        public string Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged("Company");
            }
        }

        public string Feature
        {
            get => _feature;
            set
            {
                _feature = value;
                OnPropertyChanged("Feature");
            }
        }

        public string Project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged("Project");
            }
        }

        public ObservableCollection<Table> Tables { get; } = new ObservableCollection<Table>();
    }
}
