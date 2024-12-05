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

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Key.xaml
    /// </summary>
    public partial class SystemKey : Page
    {
        public SystemKey()
        {
            InitializeComponent();
            tbKey.Text = UniqueKey.GetKey();
        }

        private void btnSchliessen_Click(object sender, RoutedEventArgs e)
        {
            MainModel model = FindResource("pmodel") as MainModel;

            model?.Navigate(model.History[^1], false);
        }
    }
}
