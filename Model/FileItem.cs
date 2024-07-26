using System.ComponentModel;
using System.IO;

namespace Klimalauf
{
    public class FileItem : INotifyPropertyChanged
    {
        private bool _isSelected = false;

        public string? FileName { get; set; }
        public DateTime? UploadDate { get; set; }

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

        public event PropertyChangedEventHandler? PropertyChanged;

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
            List<FileItem> files = new();
            Directory.CreateDirectory("Dateien");
            string[] filePaths = Directory.GetFiles("Dateien");
            foreach (string filePath in filePaths)
            {
                FileInfo fi = new(filePath);
                if (fi.Extension == ".db-shm" || fi.Extension == ".db-wal") continue;
                files.Add(new FileItem(fi.Name, fi.CreationTime));
            }
            return files;
        }


    }
}
