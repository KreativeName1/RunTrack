namespace Klimalauf
{
   public class PDFEditorModel : BaseModel
   {
      private Format _format;

      private Klasse _klasse;
      private Schueler _schueler;

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
      public Schueler Schueler
      {
         get { return _schueler; }
         set
         {
            _schueler = value;
            OnPropertyChanged("Schueler");
         }
      }


   }
}
