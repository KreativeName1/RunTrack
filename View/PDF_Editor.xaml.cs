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

            btnNeuladen.Click += (s, e) =>
            {
                PDFViewer.Navigate("about:blank");
                string pfad  = PDFGenerator.BarcodesPDF(m.Klasse, m.Klasse.Schule.Name, m.Format);
                PDFViewer.Navigate(pfad);
            };
            this.Loaded += (s, e) =>
            {
                PDFViewer.Navigate("about:blank");
                string pfad = PDFGenerator.BarcodesPDF(m.Klasse, m.Klasse.Schule.Name, m.Format);
                PDFViewer.Navigate(pfad);
            };
        }
    }
}
