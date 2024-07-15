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
using System.Windows.Shapes;

namespace Klimalauf.View
{
    /// <summary>
    /// Interaktionslogik für AdminEinstellungen.xaml
    /// </summary>
    public partial class AdminEinstellungen : Window
    {
        private string firstName;
        private string lastName;
        private string pwd;

        public AdminEinstellungen()
        {
            InitializeComponent();
        }

        public AdminEinstellungen(string firstName, string lastName, string pwd)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.pwd = pwd;
        }
    }
}
