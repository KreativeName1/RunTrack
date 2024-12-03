using System.Collections.ObjectModel;
using System.Data;

namespace RunTrack
{
    public class DBViewerModel : BaseModel
    {
        private DBService _databaseService;
        private ObservableCollection<string> _tableNames;
        private DataTable _currentTableData;
        private string _selectedTable;

        public ObservableCollection<string> TableNames
        {
            get => _tableNames;
            set { _tableNames = value; OnPropertyChanged(nameof(TableNames)); }
        }

        public DataTable CurrentTableData
        {
            get => _currentTableData;
            set { _currentTableData = value; OnPropertyChanged(nameof(CurrentTableData)); }
        }

        public string SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
                LoadTableData();
            }
        }

        private void LoadTableData()
        {
            if (!string.IsNullOrEmpty(SelectedTable))
            {
                CurrentTableData = _databaseService.GetTableData(SelectedTable);
            }
        }

        public DBViewerModel()
        {

        }

        public void Initialize(DBService service)
        {
            _databaseService = service;
            string[] blacklist =
            {
                "sqlite_sequence",
                "Benutzer"
            };
            TableNames = new ObservableCollection<string>(_databaseService.GetTableNames(blacklist));
        }
    }
}
