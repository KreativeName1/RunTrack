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

        public FileItem? SelFileItem
        {
            get
            {
                return this._selFileItem;
            }

            set
            {
                this._selFileItem = value;

                this.isFISelected = this._selFileItem != null;


                OnPropertyChanged("SelFI");
            }
        }

        private bool _isFISelected;
        public bool isFISelected
        {
            get
            {
                return _isFISelected;
            }

            set
            {
                this._isFISelected = value;
                OnPropertyChanged("IsFISelected");
            }

        }
    }
}
