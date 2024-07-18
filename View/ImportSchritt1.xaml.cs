using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für CSVImport.xaml
    /// </summary>
    public partial class ImportSchritt1 : Window
    {
        private DraggableItem _draggedItem;
        private MainViewModel _mvmodel;
        private ImportModel _imodel;


        public ImportSchritt1(string pfad)
        {
            _mvmodel = FindResource("mvmodel") as MainViewModel;
            _imodel = FindResource("imodel") as ImportModel;
            InitializeComponent();

            int width = 120;
            DraggableItem item1 = new DraggableItem { TextContent = "Vorname", Width = width };
            item1.MouseDown += DraggableItem_MouseDown;
            DraggableItem item2 = new DraggableItem { TextContent = "Nachname", Width = width };
            item2.MouseDown += DraggableItem_MouseDown;
            DraggableItem item3 = new DraggableItem { TextContent = "Geschlecht", Width = width };
            item3.MouseDown += DraggableItem_MouseDown;
            DraggableItem item4 = new DraggableItem { TextContent = "Geburtsjahrgang", Width = width };
            item4.MouseDown += DraggableItem_MouseDown;
            DraggableItem item5 = new DraggableItem { TextContent = "Klasse", Width = width };
            item5.MouseDown += DraggableItem_MouseDown;

            OrderPanel.Children.Add(item1);
            OrderPanel.Children.Add(item2);
            OrderPanel.Children.Add(item3);
            OrderPanel.Children.Add(item4);
            OrderPanel.Children.Add(item5);
            
            using (var db = new LaufDBContext())
            {
                ObservableCollection<Schule> schulen = new(db.Schulen.ToList());
                schulen.Insert(0, new Schule { Id = 0, Name = "Neue Schule" });
                _imodel.SchuleListe = schulen;
            }

            try
            {
                _imodel.CSVListe = new(CSVReader.ReadToList(pfad));
            }
            catch (FileNotFoundException) { MessageBox.Show("Datei wurde nicht gefunden."); this.Close(); }
            catch (FileLoadException) { MessageBox.Show("Datei konnte nicht geladen werden."); this.Close(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); this.Close(); }

            btnCancel.Click += (s, e) => this.Close();

            btnWeiter.Click += (s, e) =>
            {
                if (_imodel.Schule == null || _imodel.Schule.Name == "Neue Schule" && string.IsNullOrWhiteSpace(tbSchule.Text))
                {
                    MessageBox.Show("Bitte wählen Sie eine Schule aus.");
                    return;
                }

                _imodel.Reihenfolge = new();
                foreach (DraggableItem item in OrderPanel.Children) _imodel.Reihenfolge.Add(item.TextContent);
                if (_imodel.Schule.Id == 0) _imodel.Schule = new Schule { Name = tbSchule.Text };

                // weiter zur klassenerstellung
                ImportSchritt2 csvImport2 = new();
                csvImport2.Show();
                this.Close();
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
