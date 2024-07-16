using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Klimalauf
{
   /// <summary>
   /// Interaktionslogik für CSVImport.xaml
   /// </summary>
   public partial class CSVImport : Window
   {
      private DraggableItem _draggedItem;
      private MainViewModel _mvmodel;
      public CSVImport(string pfad)
      {
         _mvmodel = FindResource("mvmodel") as MainViewModel;
         InitializeComponent();


         DraggableItem item1 = new DraggableItem { TextContent = "Vorname" };
         item1.MouseDown += DraggableItem_MouseDown;
         DraggableItem item2 = new DraggableItem { TextContent = "Nachname" };
         item2.MouseDown += DraggableItem_MouseDown;
         DraggableItem item3 = new DraggableItem { TextContent = "Geschlecht" };
         item3.MouseDown += DraggableItem_MouseDown;
         DraggableItem item4 = new DraggableItem { TextContent = "Geburtsjahrgang" };
         item4.MouseDown += DraggableItem_MouseDown;
         DraggableItem item5 = new DraggableItem { TextContent = "Klasse" };
         item5.MouseDown += DraggableItem_MouseDown;

         OrderPanel.Children.Add(item1);
         OrderPanel.Children.Add(item2);
         OrderPanel.Children.Add(item3);
         OrderPanel.Children.Add(item4);
         OrderPanel.Children.Add(item5);

         using (var db = new LaufDBContext())
         {
            List<Schule> schulen = db.Schulen.ToList();
            SchoolComboBox.ItemsSource = schulen;
            SchoolComboBox.DisplayMemberPath = "Name";
            SchoolComboBox.SelectedValuePath = "Id";
         }

         //CSV_Grid.ItemsSource = CSVReader.ReadToList(pfad);

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
