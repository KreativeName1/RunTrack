using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Import1.xaml
    /// </summary>
    public partial class Import1 : Page
    {
        private DraggableItem? _draggedItem;
        private ImportModel? _imodel;


        public Import1()
        {
            _imodel = FindResource("imodel") as ImportModel ?? new ImportModel();
            InitializeComponent();

            string[] strings = { "Vorname", "Nachname", "Geschlecht", "Geburtsjahrgang", "Klasse" };
            foreach (string s in strings)
            {
                DraggableItem item = new DraggableItem { TextContent = s, Width = 120 };
                item.MouseDown += DraggableItem_MouseDown;
                OrderPanel.Children.Add(item);
            }

            using (var db = new LaufDBContext())
            {
                ObservableCollection<Schule> schulen = new(db.Schulen.ToList());
                schulen.Insert(0, new Schule { Id = 0, Name = "Neue Schule" });
                _imodel.SchuleListe = schulen;
            }

            try
            {
                _imodel.CSVListe = new(CSVReader.ReadToList(_imodel.Pfad));
            }
            catch (FileNotFoundException) { MessageBox.Show("Datei wurde nicht gefunden."); throw new Exception("Schliessen"); }
            catch (FileLoadException) { MessageBox.Show("Datei konnte nicht geladen werden."); throw new Exception("Schliessen"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); throw new Exception("Schliessen"); }

            btnCancel.Click += (s, e) =>
            {
                if (MessageBox.Show("Möchten Sie den Import wirklich abbrechen?", "Abbrechen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    _imodel.CloseWindow();
            };
            btnWeiter.Click += (s, e) =>
            {
                if (_imodel.Schule == null || _imodel.Schule.Name == "Neue Schule" && string.IsNullOrWhiteSpace(tbSchule.Text))
                {
                    MessageBox.Show("Bitte wählen Sie eine Schule aus.");
                    return;
                }

                _imodel.Reihenfolge = new();
                foreach (DraggableItem item in OrderPanel.Children) _imodel.Reihenfolge.Add(item.TextContent);
                if (_imodel.Schule.Id == 0) _imodel.Schule = new Schule { Name = _imodel.NeuSchuleName };

                // weiter zur klassenerstellung
                _imodel.ShowImport2();
            };

        }

        private void DraggableItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DraggableItem draggableItem = (DraggableItem)sender;
            DragDrop.DoDragDrop(draggableItem, draggableItem.Content, DragDropEffects.Move);
            _draggedItem = draggableItem;
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {

            if (_draggedItem != null)
            {
                OrderPanel.Children.Remove(_draggedItem);
                StackPanel panel = (StackPanel)sender;
                int index = GetCurrentMouseIndex(panel, e.GetPosition(panel));
                panel.Children.Insert(index, _draggedItem);
            }
        }

        private int GetCurrentMouseIndex(StackPanel panel, Point point)
        {
            double totalWidth = 0;
            int index = 0;
            foreach (DraggableItem item in panel.Children)
            {
                totalWidth += item.ActualWidth;
                if (totalWidth > point.X)
                {
                    return index;
                }
                index++;
            }

            return panel.Children.Count;


        }
    }
}
