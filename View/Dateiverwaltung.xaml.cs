using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Klimalauf
{
   public partial class Dateiverwaltung : Window
   {
      //public ObservableCollection<FileItem> files { get; set; }

      private MainViewModel mvmodel;

      private String firstName;
      private String lastName;

      public Dateiverwaltung(string firstName, string lastName)
      {
         InitializeComponent();

         this.firstName = firstName;
         this.lastName = lastName;

         DataContext = new MainViewModel();
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         this.mvmodel = DataContext as MainViewModel;

            mvmodel.LstFiles = new ObservableCollection<FileItem>(FileItem.AlleLesen());

        }

      private void UploadFiles_Click(object sender, RoutedEventArgs e)
      {
         OpenFileDialog openFileDialog = new OpenFileDialog
         {
            Multiselect = true,
            Filter = "files (*.asv;*.db;*.csv)|*.asv;*.db;*.csv"
         };

         if (openFileDialog.ShowDialog() == true)
         {
            foreach (string fileName in openFileDialog.FileNames)
            {
               string extension = Path.GetExtension(fileName).ToLower();

                    if (extension == ".asv" || extension == ".db" || extension == ".csv")
                    {
                        mvmodel.LstFiles.Add(new FileItem
                        {
                            fileName = Path.GetFileName(fileName),
                            uploadDate = DateTime.Now
                        });
                        Directory.CreateDirectory("Dateien");
                        string destPath = Path.Combine("Dateien", Path.GetFileName(fileName));
                        File.Copy(fileName, destPath, true);
                    }
                    else
                    {
                  MessageBox.Show($"Die Datei {fileName} hat eine ungültige Erweiterung und kann nicht hochgeladen werden.");
               }
            }
         }
      }

      private void DeleteSelectedFiles_Click(object sender, RoutedEventArgs e)
      {
         var selectedFiles = mvmodel.LstFiles.Where(f => f.IsSelected).ToList();
         foreach (var file in selectedFiles)
         {
            mvmodel.LstFiles.Remove(file);
         }
         SelectAllCheckBox.IsChecked = false;
      }

      private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
      {
         bool newValue = (SelectAllCheckBox.IsChecked == true);
         foreach (var file in mvmodel.LstFiles)
         {
            file.IsSelected = newValue;
         }

         SelectAllTextBlock.Text = SelectAllCheckBox.IsChecked == true ? "Deselect All" : "Select All";
      }

      private void CloseWindow_Click(object sender, RoutedEventArgs e)
      {
         // Open admin panel window
         AdminScanner adminPanel = new AdminScanner(firstName, lastName);
         adminPanel.Show();
         this.Close();
      }

      private void DownloadFiles_Click(object sender, RoutedEventArgs e)
      {

      }
   }
}
