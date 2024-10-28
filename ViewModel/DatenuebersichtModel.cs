using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace RunTrack
{
    public class DatenuebersichtModel : BaseModel
    {
        private Page? _currentPage;

        private bool _hasChanges;

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
