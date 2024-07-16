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
        private MainViewModel _mvmodel;
        public CSVImport(string pfad)
        {
            _mvmodel = FindResource("mvmodel") as MainViewModel;
            InitializeComponent();
            TextBlock item1 = new TextBlock { Text = "Item 1", Background= Brushes.Aqua};
            item1.MouseDown += TextBlock_MouseDown;
            TextBlock item2 = new TextBlock { Text = "Item 2", Background = Brushes.IndianRed };
            item2.MouseDown += TextBlock_MouseDown;
            TextBlock item3 = new TextBlock { Text = "Item 3", Background = Brushes.DarkOrange };
            item3.MouseDown += TextBlock_MouseDown;

            OrderPanel.Children.Add(item1);
            OrderPanel.Children.Add(item2);
            OrderPanel.Children.Add(item3);

        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            DragDrop.DoDragDrop(textBlock, textBlock.Text, DragDropEffects.Move);
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            // insert the item at the between the two items where the drop happened
            TextBlock textBlock = new TextBlock { Text = (string)e.Data.GetData(DataFormats.Text) };
            textBlock.MouseDown += TextBlock_MouseDown;
            StackPanel stackPanel = (StackPanel)sender;
            stackPanel.Children.Insert(stackPanel.Children.IndexOf((UIElement)e.Source), textBlock);
        }
    }
}
