using System.Collections.ObjectModel;

namespace Klimalauf
{
   public class MainViewModel : BaseModel
   {
      private ObservableCollection<FileItem> _lstFiles;
      private ObservableCollection<ScanItem> _lstScanner;

      public ObservableCollection<ScanItem> LstScanner
      {
         get
         {
            return this._lstScanner;
         }
         set
         {
            this._lstScanner = value;
            OnPropertyChanged("lstScanner");
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
            OnPropertyChanged("lstFiles");
         }
      }

      private Scanner _selSC;
      public Scanner selSC
      {
         get
         {
            return this._selSC;
         }

         set
         {
            this._selSC = value;

            this.isSCSelected = this._selSC != null;


            OnPropertyChanged("SelSC");
         }
      }

      private Scanner _selFI;
      public Scanner selFI
      {
         get
         {
            return this._selFI;
         }

         set
         {
            this._selFI = value;

            this.isFISelected = this._selFI != null;


            OnPropertyChanged("SelFI");
         }
      }

      private bool _isSCSelected;
      public bool isSCSelected
      {
         get
         {
            return _isSCSelected;
         }

         set
         {
            this._isSCSelected = value;
            OnPropertyChanged("IsSCSelected");
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

      public MainViewModel()
      {
         // der Konstruktor der ObservableCollection nimmt eine List<...> und
         // wandelt diese um

         this.LstScanner = new ObservableCollection<ScanItem>(ScanItem.AlleLesen());
         this.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
      }
   }
}
