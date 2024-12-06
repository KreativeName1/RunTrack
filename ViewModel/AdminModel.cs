using System.Collections.ObjectModel;

namespace RunTrack
{
    // Definiert die AdminModel-Klasse, die von BaseModel erbt
    internal class AdminModel : BaseModel
    {
        // Private Variable für die Liste der Benutzer
        private ObservableCollection<Benutzer> _LstBenutzer;

        // Öffentliche Eigenschaft für die Liste der Benutzer
        public ObservableCollection<Benutzer> LstBenutzer
        {
            get { return _LstBenutzer; }
            set
            {
                _LstBenutzer = value;
                // Benachrichtigt über die Änderung der Eigenschaft
                OnPropertyChanged("LstBenutzer");
            }
        }

        // Private Variable für das ausgewählte Datum, initialisiert mit dem aktuellen Datum
        private DateTime _selDate = DateTime.Now;

        // Private Variable für den ausgewählten Benutzer
        private Benutzer _SelBenutzer;

        // Öffentliche Eigenschaft für den ausgewählten Benutzer
        public Benutzer SelBenutzer
        {
            get { return _SelBenutzer; }
            set
            {
                _SelBenutzer = value;
                // Benachrichtigt über die Änderung der Eigenschaft
                OnPropertyChanged("SelBenutzer");
            }
        }

        // Konstruktor der AdminModel-Klasse
        public AdminModel()
        {
            // Erstellt eine neue Instanz des Datenbankkontexts
            using (LaufDBContext db = new())
            {
                // Initialisiert die Liste der Benutzer mit den Daten aus der Datenbank
                this.LstBenutzer = new ObservableCollection<Benutzer>(db.Benutzer.ToList());
            }
        }
    }
}
