﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für SchuelerSeite.xaml
    /// </summary>
    public partial class SchuelerSeite : Page
    {
        private SchuelerseiteModel _model;
        private bool _isUserInteraction = false;
        private bool _isUserInteractionGeschlecht = false;

        public SchuelerSeite()
        {
            InitializeComponent();
            _model = FindResource("thismodel") as SchuelerseiteModel;

            this.Unloaded += (s, e) =>
            {
                _model.Db.Dispose();
                _model.HasChanges = false;
            };

            btnNeu.Click += (s, e) =>
            {
                txtSearch.Text = "";
                txtSearch.IsEnabled = false;

                var neuerSchueler = new Schueler();
                neuerSchueler.Geburtsjahrgang = 2000;

                _model.LstSchueler.Add(neuerSchueler);
                _model.SelSchueler = neuerSchueler; // Setze den neuen Schüler als ausgewählt
                lstSchueler.SelectedItem = neuerSchueler; // Stelle sicher, dass er im DataGrid ausgewählt ist
                lstSchueler.ScrollIntoView(neuerSchueler); // Scrolle zum neuen Eintrag

                // Fokus auf das DataGrid setzen
                lstSchueler.Focus();

                // Dispatcher verwenden, um die Bearbeitung zu aktivieren, nachdem alle Ereignisse verarbeitet sind
                Dispatcher.InvokeAsync(() =>
                {
                    var firstEditableColumn = lstSchueler.Columns.FirstOrDefault(col => !col.IsReadOnly);
                    if (firstEditableColumn != null)
                    {
                        lstSchueler.CurrentCell = new DataGridCellInfo(neuerSchueler, firstEditableColumn);
                        lstSchueler.BeginEdit();
                    }
                });

                _model.HasChanges = true;
            };

            btnSpeichern.Click += (s, e) =>
            {
                txtSearch.IsEnabled = true;

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
                string message = "";

                if (_model.SelSchueler.Id == null)
                {
                    message = "Möchten Sie diesen Eintrag wirklich löschen?";
                }
                else
                {
                    message = $"Möchten Sie diesen Eintrag wirklich löschen? \n- {_model.SelSchueler.Id}:\t{_model.SelSchueler.Nachname}, {_model.SelSchueler.Vorname}";
                }

                var res = new Popup().Display("Löschen", message, PopupType.Question, PopupButtons.YesNo);

                if (res == true)
                {
                    _model.LstSchueler.Remove(_model.SelSchueler);
                    _model.HasChanges = true;
                }
            };

            lstSchueler.CellEditEnding += (s, e) =>
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    if (_model.SelSchueler.Geburtsjahrgang < 1900)
                    {
                        _model.SelSchueler.Geburtsjahrgang = 1900;
                    }
                    _model.HasChanges = true;
                }
            };
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, false);
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            UebersichtMethoden.SelectSearchedRow(lstSchueler, true);
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.ForegroundBrush = new SolidColorBrush(Colors.Blue);
            txtSearch.Foreground = new SolidColorBrush(Colors.Blue);
        }


        private void cbKlasse_DropDown(object sender, EventArgs e)
        {
            _isUserInteraction = !_isUserInteraction;
        }


        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            _isUserInteractionGeschlecht = false;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteractionGeschlecht)
            {
                _model.HasChanges = true;
                ComboBox cb = sender as ComboBox;
                Schueler schueler = cb.DataContext as Schueler;
                if (schueler != null)
                {
                    schueler.Geschlecht = (Geschlecht)cb.SelectedItem;
                }
            }
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            _isUserInteraction = true;
        }

        private void comboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (_isUserInteraction)
            {
                _model.HasChanges = true;
                ComboBox cb = sender as ComboBox;
                Schueler schueler = cb.DataContext as Schueler;

                // find schueler in the LstSchueler and set klasse

                if (schueler != null)
                {
                    Schueler inListe = _model.LstSchueler.FirstOrDefault(s => s.Id == schueler.Id);
                    if (inListe != null)
                    {
                        inListe.Klasse = (Klasse)cb.SelectedItem;
                        inListe.KlasseId = inListe.Klasse.Id;
                    }
                }
            }
        }


        private void comboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is Schueler schueler)
            {
                Trace.WriteLine(_model.LstSchueler);
                ListCollectionView view = new(_model.LstKlasse);
                view.GroupDescriptions.Add(new PropertyGroupDescription("Schule.Name"));
                comboBox.ItemsSource = view;

                if (schueler.Klasse != null)
                {
                    comboBox.SelectedItem = schueler.Klasse;
                }
            }
        }
    }
}