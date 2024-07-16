using System.ComponentModel;

namespace Klimalauf
{
   public class FileItem : INotifyPropertyChanged
   {
      private bool _isSelected;

      public string FileName { get; set; }
      public DateTime UploadDate { get; set; }

      public bool IsSelected
      {
         get => _isSelected;
         set
         {
            if (_isSelected != value)
            {
               _isSelected = value;
               OnPropertyChanged("IsSelected");
            }
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;

      protected void OnPropertyChanged(string propertyName)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      public FileItem() { }

      public FileItem(string fileName, DateTime uploadDate)
      {
         FileName = fileName;
         UploadDate = uploadDate;
      }

      public static List<FileItem> AlleLesen()
      {
         return DBFile.AlleLesen();
      }


   }
}
