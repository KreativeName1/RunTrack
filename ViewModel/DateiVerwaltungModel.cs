using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTrack
{
    internal class DateiVerwaltungModel : BaseModel
    {
        private ObservableCollection<FileItem> _lstFiles = new();
        private FileItem? _selFileItem;
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
