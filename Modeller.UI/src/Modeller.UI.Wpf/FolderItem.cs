using System.Collections;

namespace Modeller.UI.Wpf
{
    /// <summary>
    /// Provides a virtual folder data structure for arbitrary
    /// child items.
    /// </summary>
    public class FolderItem : SimpleObject
    {
        private string _name;
        private IEnumerable _items;

        /// <summary>
        /// The name that can be displayed or used as an
        /// ID to perform more complex styling.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                //ignore if values are equal
                if (value == _name)
                    return;

                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The child items of the folder.
        /// </summary>
        public IEnumerable Items
        {
            get => _items;
            set
            {
                //ignore if values are equal
                if (value == _items)
                    return;

                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public FolderItem()
        {
        }

        /// <summary>
        /// This method is invoked by WPF to render the object if
        /// no data template is available.
        /// </summary>
        /// <returns>Returns the value of the <see cref="Name"/>
        /// property.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", GetType().Name, Name);
        }
    }
}
