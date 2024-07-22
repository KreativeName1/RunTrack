using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für SearchBar.xaml
    /// </summary>
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            InitializeComponent();
        }

        // Event für den Search-Button-Klick
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            OnSearchRequested(searchText);
        }

        // Delegat und Event für die Suchanfrage
        public delegate void SearchRequestedEventHandler(object sender, string searchText);
        public event SearchRequestedEventHandler SearchRequested;

        protected virtual void OnSearchRequested(string searchText)
        {
            SearchRequested?.Invoke(this, searchText);
        }
    }
}
