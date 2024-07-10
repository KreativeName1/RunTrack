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
using iText;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für PDF_Editor.xaml
    /// </summary>
    public partial class PDF_Editor : Window
    {
        PDFEditorModel? m;
        string? pfad;

        public PDF_Editor(Klasse klasse) : base()
        {
            InitializeComponent();
            m = FindResource("pemodel") as PDFEditorModel;
            m.Klasse = klasse;
            m.Format = new Format();
            cbTyp.ItemsSource =  Enum.GetValues(typeof(SchriftTyp)); 
           

            cbBlattgroessee.ItemsSource = new List<string> { "A4", "A5", "A6" };

            using (var db = new LaufDBContext())
            {
                cbFormate.ItemsSource = db.Formate.ToList();
            }

            btnNeuladen.Click += (s, e) =>
            {

                webView.Source = new Uri("about:blank");
                string pfad  = PDFGenerator.BarcodesPDF(m.Klasse, m.Klasse.Schule.Name, m.Format);
                webView.Source = new Uri(pfad);
            };
            this.Loaded += (s, e) =>
            {
                webView.Source = new Uri("about:blank");
                string pfad = PDFGenerator.BarcodesPDF(m.Klasse, m.Klasse.Schule.Name, m.Format);
                webView.Source = new Uri(pfad);
            };

            btnSpeichern.Click += (s, e) =>
            {
                using (var db = new LaufDBContext())
                {
                    if (db.Formate.Any(f => f.Name == m.Format.Name))
                    {
                        MessageBoxResult result = MessageBox.Show("Format überschreiben?", "Format existiert bereits", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.No) return;
                        db.Formate.Update(m.Format);
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Neues Format speichern?", "Format speichern", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.No) return;
                        m.Format.Id = 0;
                        db.Formate.Add(m.Format);
                    }
                    db.SaveChanges();
                    cbFormate.ItemsSource = db.Formate.ToList();
                }
            };

            cbFormate.SelectionChanged += (s, e) =>
            {
                m.Format = (Format)cbFormate.SelectedItem;
                cbTyp.SelectedIndex = m.Format.SchriftTyp switch
                {
                    SchriftTyp.Normal => 0,
                    SchriftTyp.Fett => 1,
                    SchriftTyp.Kursiv => 2,
                    SchriftTyp.FettKursiv => 3,
                    _ => 0
                };
            };
            cbTyp.SelectionChanged += (s, e) =>
            {
                m.Format.SchriftTyp = (SchriftTyp)cbTyp.SelectedIndex;
            };
        }

    }
}
