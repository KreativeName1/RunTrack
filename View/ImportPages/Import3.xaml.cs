using System.Windows.Controls;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Import3.xaml
    /// </summary>
    public partial class Import3 : Page
    {
        private ImportModel _imodel;
        public Import3()
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new();
            DataContext = _imodel;

            btnSchliessen.Click += (s, e) =>
            {
                if (tbTitel.Text == "Fehler") _imodel.ShowImport2();
                else _imodel.CloseWindow();
            };
            try
            {
                ImportIntoDB importIntoDB = new(_imodel);
                tbTitel.Text = "Erfolg";
                tbMeldung.Text = "Import war erfolgreich.";
                tbTitel.Foreground = Brushes.Green;
                btnSchliessen.Content = "Schließen";
            }
            catch (ImportException ex)
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = ex.Message;
                btnSchliessen.Content = "Zurück";
            }
            catch (Exception)
            {
                tbTitel.Foreground = Brushes.Red;
                tbTitel.Text = "Fehler";
                tbMeldung.Text = "Ein unerwarteter Fehler ist aufgetreten.";
                btnSchliessen.Content = "Zurück";
            }
        }
    }
}
