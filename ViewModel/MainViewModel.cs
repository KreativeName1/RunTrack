using System.Collections.ObjectModel;

namespace Klimalauf
{
   public class MainViewModel : BaseModel
   {
      private ObservableCollection<FileItem> _lstFiles = new ObservableCollection<FileItem>();
      private ObservableCollection<Runde> _lstRunden = new ObservableCollection<Runde>();
      private ObservableCollection<Runde> _lstLetzteRunde = new ObservableCollection<Runde>();

      public ObservableCollection<Runde> LstRunden
      {
         get
         {
            return this._lstRunden;
         }
         set
         {
            this._lstRunden = value;
            OnPropertyChanged("LstRunden");
         }
      }

      public ObservableCollection<Runde> LstLetzteRunde
      {
         get
         {
            return this._lstLetzteRunde;
         }
         set
         {
            this._lstLetzteRunde = new ObservableCollection<Runde>();
            this._lstLetzteRunde = value;
            OnPropertyChanged("LstLetzteRunden");
         }
      }

      public Runde ZulestGescannt
      {
         get
         {
            return LstRunden.Last();
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

      private ScanItem _selSC;
      public ScanItem selSC
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

      private FileItem _selFI;
      public FileItem selFI
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

      //public MainViewModel()
      //{
      //   this.LstScanner = new ObservableCollection<ScanItem>(ScanItem.AlleLesen());
      //   this.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());
      //}
   }
}
