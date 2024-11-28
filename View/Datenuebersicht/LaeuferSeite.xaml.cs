using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class LaeuferSeite : Page
    {
        private LaeuferseiteModel _model;
        private MainModel _mmodel;
        private bool _isUserInteraction = false;

        public LaeuferSeite()
        {
            InitializeComponent();

            _model = FindResource("thismodel") as LaeuferseiteModel;
            _mmodel = FindResource("pmodel") as MainModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };
            btnNeu.Click += (s, e) =>
            {
                _model.LstLaeufer.Add(new Laeufer());
                _model.HasChanges = true;
            };
            btnSpeichern.Click += (s, e) =>
            {
                try
                {
                    _model.SaveChanges();
                }
                catch (Exception ex)
                {
                    new Popup().Display("Fehler beim Speichern", ex.Message, PopupType.Error, PopupButtons.Ok);
                }
            };
            btnDel.Click += (s, e) =>
            {
                _model.LstLaeufer.Remove(_model.SelLaeufer);
                _model.HasChanges = true;
            };
            btnDruck.Click += (s, e) =>
            {
                //// get all selected Laeufer
                List<Laeufer> list = new();

                foreach (Laeufer laeufer in lstLaeufer.SelectedItems)
                {
                    list.Add(laeufer);
                }


                PDFEditor editor = new(list);
                _mmodel.Navigate(editor);




                
            };
            lstLaeufer.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit) _model.HasChanges = true;
            };


        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstLaeufer, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }


        private void cbRundenArt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model.SelLaeufer == null) return;
            ComboBox cbRundenArt = sender as ComboBox;
            _model.HasChanges = true;
            RundenArt rundenArt = cbRundenArt.SelectedItem as RundenArt;

            if (rundenArt != null)
            {
                _model.SelLaeufer.RundenArt = rundenArt;
                _model.SelLaeufer.RundenArtId = rundenArt.Id;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteraction)
            {
                _model.HasChanges = true;

                ComboBox cb = sender as ComboBox;
                Laeufer laeufer = cb.DataContext as Laeufer;

                if (laeufer != null)
                {
                    laeufer.Geschlecht = (Geschlecht)cb.SelectedItem;
                }
            }
        }

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            _isUserInteraction = !_isUserInteraction;
        }
    }
}
