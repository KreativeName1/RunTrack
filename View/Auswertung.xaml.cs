using Google.Protobuf.WellKnownTypes;
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

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Auswertung.xaml
    /// </summary>
    public partial class Auswertung : Window
    {
        private string[] pfade;
        public Auswertung()
        {
            InitializeComponent();
            pfade = System.IO.Directory.GetFiles("Dateien", "*.db");
            LoadData();
        }

        public void LoadData()
        {
            using (var db = new MergedDBContext(pfade))
            {
                bool first = true;
                foreach (RundenArt rundenArt in db.RundenArten)
                {
                    RadioButton rb = new RadioButton
                    {
                        Content = rundenArt.Name,
                        Name = rundenArt.Name.Replace(" ", "_"),
                        IsChecked = first
                    };
                    RundenGroesse.Children.Add(rb);
                    first = false;
                }

                cbSchule.ItemsSource = db.Schulen.ToList();
                cbSchule.SelectedIndex = 0;
                cbKlasse.ItemsSource = db.Klassen.ToList();
                cbKlasse.SelectedIndex = 0;
                iudJahrgang.Value = 0;

            }
        }
    }
}
