using System.Collections.ObjectModel;

namespace Klimalauf
{
   public class PDFEditorModel : BaseModel
   {
      private Format _format;

      private Klasse _klasse;
      private ObservableCollection<Schueler> _schueler;

        private bool _neueSeiteProSchueler = true;

      public Format Format
      {
         get { return _format; }
         set
         {
            _format = value;
            OnPropertyChanged("Format");
         }
      }

        

      public Klasse Klasse
      {
         get { return _klasse; }
         set
         {
            _klasse = value;
            OnPropertyChanged("Klasse");
         }
      }
      public ObservableCollection<Schueler> Schueler
      {
         get { return _schueler; }
         set
         {
            _schueler = value;
            OnPropertyChanged("Schueler");
         }
      }


        public bool NeueSeiteProSchueler
        {
            get { return _neueSeiteProSchueler; }
            set
            {
                _neueSeiteProSchueler = value;
                OnPropertyChanged("NeueSeiteProSchueler");
            }
        }

    }
}
