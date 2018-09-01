using System.Windows;
using Microsoft.Win32;
using Modeller.Extensions;
using Modeller.Models;

namespace Modeller.UI.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenModel(string filename)
        {
            string model;
            using (var reader = System.IO.File.OpenText(filename))
            {
                model = reader.ReadToEnd();
            }
            var module = model.FromJson<Module>();

            var s = new Database { Company = module.Company, Feature = module.Feature?.Singular.Value, Project = module.Project.Singular.Value };
            foreach (var item in module.Models)
            {
                var table = new Table(s) { Name = item.Name.Singular.Value, IsAuditable = item.HasAudit, SupportActive = item.HasActive() };
                foreach (var field in item.Key.Fields)
                {
                    table.Key.Add(new Column(table) { Name = field.Name.Singular.Value, IsBusinessKey = field.BusinessKey, DataType = field.DataType, Decimals = field.Decimals, Default = field.Default, MaxLength = field.MaxLength, MinLength = field.MinLength, IsNullable = field.Nullable });
                }
                foreach (var field in item.Fields)
                {
                    table.Columns.Add(new Column(table) { Name = field.Name.Singular.Value, IsBusinessKey = field.BusinessKey, DataType = field.DataType, Decimals = field.Decimals, Default = field.Default, MaxLength = field.MaxLength, MinLength = field.MinLength, IsNullable = field.Nullable });
                }
                foreach (var relationship in item.Relationships)
                {
                    var index = new Index(table)
                    {
                        LeftColumn = relationship.LeftField.Singular.Value,
                        LeftTable = relationship.LeftModel.Singular.Value,
                        LeftType = relationship.LeftType,
                        RightColumn = relationship.RightField.Singular.Value,
                        RightTable = relationship.RightModel.Singular.Value,
                        RightType = relationship.RightType
                    };
                    table.Relationships.Add(index);
                }
                s.Tables.Add(table);
            }

            tvwModel.Items.Add(s);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            tvwModel.Items.Clear();
            AddDatabase();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                OpenModel(openFileDialog.FileName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
        }

        public Database SelectedDatabase
        {
            get => (Database)GetValue(SelectedDatabaseProperty);
            set => SetValue(SelectedDatabaseProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedDatabase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDatabaseProperty =
            DependencyProperty.Register("SelectedDatabase", typeof(Database), typeof(MainWindow));

        public Table SelectedTable
        {
            get => (Table)GetValue(SelectedTableProperty);
            set => SetValue(SelectedTableProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTableProperty =
            DependencyProperty.Register("SelectedTable", typeof(Table), typeof(MainWindow), new PropertyMetadata(null));

        public Column SelectedColumn
        {
            get => (Column)GetValue(SelectedColumnProperty);
            set => SetValue(SelectedColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedField.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColumnProperty =
            DependencyProperty.Register("SelectedColumn", typeof(Column), typeof(MainWindow), new PropertyMetadata(null));

        public Index SelectedIndex
        {
            get => (Index)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(Index), typeof(MainWindow), new PropertyMetadata(null));

        private void tvwModel_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Index i)
            {
                SelectedColumn = null;
                SelectedIndex = i;
                SelectedTable = i?.Parent;
                SelectedDatabase = i?.Parent?.Parent;
            }
            else if (e.NewValue is Column f)
            {
                SelectedIndex = null;
                SelectedColumn = f;
                SelectedTable = f?.Parent;
                SelectedDatabase = f?.Parent?.Parent;
            }
            if (e.NewValue is Table t)
            {
                if (SelectedTable != t)
                {
                    SelectedTable = t;
                    SelectedDatabase = t?.Parent;
                    SelectedColumn = null;
                    SelectedIndex = null;
                }
            }
            if (e.NewValue is Database d)
            {
                if (SelectedDatabase != d)
                {
                    SelectedDatabase = d;
                    SelectedTable = null;
                    SelectedColumn = null;
                    SelectedIndex = null;
                }
            }
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            SelectedColumn = null;
            SelectedIndex = null;
            var table = new Table(SelectedDatabase);
            SelectedDatabase.Tables.Add(table);
            SelectedTable = table;
        }

        private void AddDatabase()
        {
            SelectedDatabase = new Database() { Company = Defaults.CompanyName };
            SelectedDatabase.Tables.Add(new Wpf.Table(SelectedDatabase));

            tvwModel.Items.Add(SelectedDatabase);
        }

        private void AddDatabase_Click(object sender, RoutedEventArgs e) => AddDatabase();
    }
}