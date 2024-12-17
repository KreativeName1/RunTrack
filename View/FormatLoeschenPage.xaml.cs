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
    /// Interaktionslogik für FormatLoeschenPage.xaml
    /// </summary>
    public partial class FormatLoeschenPage : Page
    {
        private PDFEditorModel _pemodel;

        public FormatLoeschenPage()
        {
            InitializeComponent();
            _pemodel = new PDFEditorModel();
            _pemodel.LoadData();
            DataContext = _pemodel;
        }

        private void Loeschen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int formatId)
            {
                var format = _pemodel.Formate?.FirstOrDefault(f => f.Id == formatId);
                if (format != null)
                {
                    using (var db = new LaufDBContext())
                    {
                        db.Formate.Remove(format);
                        db.SaveChanges();
                    }
                    _pemodel.Formate?.Remove(format);
                }
            }
        }
    }
}
