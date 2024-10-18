using System.Collections.ObjectModel;

namespace RunTrack
{
    internal class DateiVerwaltungModel : BaseModel
    {
        private ObservableCollection<FileItem> _lstFiles = new();
        private bool _isDragging = false;

        public bool IsDragging
        {

            get
            {
                return _isDragging;
            }
            set
            {
                _isDragging = value;
                OnPropertyChanged("IsDragging");
            }
        }

        public ObservableCollection<FileItem> LstFiles
        {
            get
            {
                return this._lstFiles;
            }
            set
            {

                this._lstFiles = value;
                OnPropertyChanged("LstFiles");
            }
        }
    }
}
