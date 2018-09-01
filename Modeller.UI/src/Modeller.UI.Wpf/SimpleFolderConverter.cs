using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Modeller.UI.Wpf
{
    public class NullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A converter that organizes several collections into (optional)
    /// child collections that are put into <see cref="FolderItem"/>
    /// containers.
    /// </summary>
    public class SimpleFolderConverter : IMultiValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //get folder name listing...
            var folder = parameter as string ?? "";
            var folders = folder.Split(',').Select(f => f.Trim()).ToList();
            //...and make sure there are no missing entries
            while (values.Length > folders.Count)
                folders.Add(string.Empty);

            //this is the collection that gets all top level items
            var items = new List<object>();

            for (var i = 0; i < values.Length; i++)
            {
                //make sure were working with collections from here...
                var childs = values[i] as IEnumerable ?? new List<object> { values[i] };

                var folderName = folders[i];
                if (folderName != string.Empty)
                {
                    if(folderName=="Key")
                    {
                        foreach (var item in childs)
                        {
                            ((Column)item).Image = "images\\key.png";
                        }
                    }

                    //create folder item and assign childs
                    var folderItem = new FolderItem { Name = folderName, Items = childs };
                    items.Add(folderItem);
                }
                else
                {
                    //if no folder name was specified, move the item directly to the root item
                    foreach (var child in childs)
                    { items.Add(child); }
                }
            }

            return items;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot perform reverse-conversion");
        }
    }
}
