using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für CSVImport.xaml
    /// </summary>
    public partial class CSVImport : Window
    {
        public ObservableCollection<string> StackPanelItems { get; set; }
        private MainViewModel _mvmodel;
        public CSVImport()
        {
            _mvmodel = FindResource("mvmodel") as MainViewModel;
            InitializeComponent();
            StackPanelItems = new ObservableCollection<string> { "Item 1", "Item 2", "Item 3" };
            Leiste.Benutzername = _mvmodel.Benutzer.Vorname + ", " + _mvmodel.Benutzer.Nachname;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TextBlock draggedTextBlock = (TextBlock)sender;
                DataObject dataObject = new DataObject();
                dataObject.SetData("OriginalIndex", StackPanelItems.IndexOf(draggedTextBlock.Text));
                DragDrop.DoDragDrop(draggedTextBlock, dataObject, DragDropEffects.Move);
            }
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("OriginalIndex"))
            {
                int originalIndex = (int)e.Data.GetData("OriginalIndex");
                int newIndex = GetNewIndexFromMousePosition(e.GetPosition(OrderPanel), sender); // Implement logic to determine new index based on mouse position

                if (originalIndex != newIndex && originalIndex >= 0 && newIndex < StackPanelItems.Count)
                {
                    string draggedText = StackPanelItems[originalIndex];
                    StackPanelItems.RemoveAt(originalIndex);
                    StackPanelItems.Insert(newIndex, draggedText);
                }
            }
        }

        private int GetNewIndexFromMousePosition(Point point, object sender)
        {
            double totalWidth = OrderPanel.ActualWidth;
            int newIndex = 0;

            foreach (TextBlock child in OrderPanel.Children)
            {
                if (child != sender && point.X < child.ActualWidth + child.TranslatePoint(new Point(0, 0), OrderPanel).X)
                {
                    break;
                }
                newIndex++;
            }

            return Math.Min(newIndex, StackPanelItems.Count - 1);
        }
    }
}
