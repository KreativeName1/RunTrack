using System.Windows.Controls;

namespace RunTrack
{
    public class DatenuebersichtModel : BaseModel
    {
        private Page? _currentPage;

        private bool _hasChanges;

        private bool _readOnly = false;

        private string? connectionString;

        public string? ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                this.connectionString = value;
                OnPropertyChanged("ConnectionString");
            }
        }

        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                this._readOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        public bool HasChanges
        {
            get
            {
                return _hasChanges;
            }
            set
            {
                this._hasChanges = value;
                OnPropertyChanged("HasChanges");
            }
        }

        public Page? CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                this._currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }
    }
}
