using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RunTrack
{
    public class DBViewerModel : BaseModel
    {

        private ObservableCollection<TabItem> _tabs = new();
        private SqliteConnection _con;
        private string _path;

        public ObservableCollection<TabItem> Tabs
        {
            get { return _tabs; }
            set { _tabs = value; OnPropertyChanged("Tabs"); }
        }

        public DBViewerModel()
        {

        }


        public void Initialize(string path) {
            _path = path;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            Tabs = new();
            
            string connectionString = $"Data Source={_path}";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Get a list of all table names
                string query = "SELECT name FROM sqlite_master WHERE type='table'";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        TabItem tabItem = new();
                        tabItem.Header = tableName;

                        ObservableCollection<ExpandoObject> data = new();

                        // Get the table's content
                        query = $"SELECT * FROM {tableName}";
                        using (SqliteCommand tableCommand = new SqliteCommand(query, connection))
                        {
                            SqliteDataReader tableReader = tableCommand.ExecuteReader();
                            while (tableReader.Read())
                            {
                                // Create a new object for the current row
                                dynamic rowObject = new ExpandoObject();
                                for (int i = 0; i < tableReader.FieldCount; i++)
                                {
                                    ((IDictionary<string, object>)rowObject).Add(tableReader.GetName(i), tableReader.GetValue(i));
                                }
                                data.Add(rowObject);

                            }
                        }

                        DataGrid dataGrid = new()
                        {
                            ItemsSource = data
                        };

                        tabItem.Content = dataGrid;

                                    Tabs.Add(tabItem);
                    }
                }
            }

            //using (var con = new SqliteConnection(connectionString))
            //{
            //    con.Open();
            //    _con = con;
            //    var command = con.CreateCommand();
            //    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
            //    using (var reader = command.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            TabItem tabItem = new();
            //            tabItem.Header = reader.GetString(0);

            //            ObservableCollection<object> data = new();
            //            var command2 = con.CreateCommand();
            //            command2.CommandText = $"SELECT * FROM {reader.GetString(0)}";
            //            using (var reader2 = command2.ExecuteReader())
            //            {
            //                while (reader2.Read())
            //                {

            //                    foreach (var field in reader2)
            //                    {
            //                        data.Add(field);
            //                    }
            //                }
            //            }

            //            DataGrid dataGrid = new()
            //            {
            //                ItemsSource = data
            //            };


            //            tabItem.Content = dataGrid;
            //            Tabs.Add(tabItem);
            //        }
            //    }
            //}


            //foreach (var entityType in entityTypes)
            //{
            //    TabItem tabItem = new();
            //    tabItem.Header = entityType.DisplayName();

            //    DataGrid dataGrid = new()
            //    {
            //    };

            //    tabItem.Content = dataGrid;
            //    Tabs.Add(tabItem);
            //}
        }
    }
}
