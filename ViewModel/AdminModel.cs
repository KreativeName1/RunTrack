using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTrack
{
    internal class AdminModel : BaseModel
    {
        private ObservableCollection<Benutzer> _LstBenutzer;
        public ObservableCollection<Benutzer> LstBenutzer
        {
            get { return _LstBenutzer; }
            set
            {
                _LstBenutzer = value;
                OnPropertyChanged("LstBenutzer");
            }
        }

        private DateTime _selDate = DateTime.Now;


        private Benutzer _SelBenutzer;
        public Benutzer SelBenutzer
        {
            get { return _SelBenutzer; }
            set
            {
                _SelBenutzer = value;
                OnPropertyChanged("SelBenutzer");
            }
        }

        public AdminModel()
        {
            using (LaufDBContext db = new())
            {
                this.LstBenutzer = new ObservableCollection<Benutzer>(db.Benutzer.ToList());
            }
        }
    }
}
