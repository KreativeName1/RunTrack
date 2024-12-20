using System.Data.Entity.Migrations.Design;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für ImportUbersicht.xaml
    /// </summary>
    public partial class ImportUbersicht : Page
    {
        ImportModel? _imodel;
        MainModel? _model;
        string _tempPath = MainModel.BaseFolder + "/Temp";
        string _dateienPath = MainModel.BaseFolder + "/Dateien";
        public ImportUbersicht()
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();

            // Event-Handler für das Loaded-Ereignis der Seite
            this.Loaded += (s, e) =>
            {
                // Verstecke das Lade-Overlay
                LoadOverlay.Visibility = System.Windows.Visibility.Hidden;
                try
                {
                    // 1. Lösche Temp/Temp.db, falls vorhanden
                   
                    if (File.Exists($"{_tempPath}/Temp.db")) File.Delete($"{_tempPath}/Temp.db");

                    // 2. Kopiere EigeneDatenbank.db zu Temp/Temp.db
                    File.Copy($"{_dateienPath}/EigeneDatenbank.db", $"{_tempPath}/Temp.db", true);

                    // 3. Import-Daten in die temporäre Datenbank einfügen
                    ImportIntoDB importIntoDB = new ImportIntoDB(_imodel, $"{_tempPath}/Temp.db");
                }
                catch (Exception ex)
                {
                    // Navigiere zu einer Fehlerseite, falls ein Fehler auftritt
                    _model?.Navigate(new Import3(ex.Message, false));
                    return;
                }

                // 4. Daten in der Datenübersicht anzeigen
                try
                {
                    Datenuebersicht datenuebersicht = new();
                    datenuebersicht.btnSchliessen.Visibility = System.Windows.Visibility.Collapsed;
                    ubersicht.Content = datenuebersicht;
                    DatenuebersichtModel dumodel = FindResource("dumodel") as DatenuebersichtModel ?? new();
                    dumodel.ReadOnly = true;
                    dumodel.ConnectionString = Path.Combine(MainModel.BaseFolder, "Temp/Temp.db");
                }
                catch (Exception ex)
                {
                    // Navigiere zu einer Fehlerseite, falls ein Fehler auftritt
                    _model?.Navigate(new Import3(ex.Message, false));
                    return;
                }

                // 5. Wenn Bestätigen geklickt wird, verschiebe Temp/Temp.db zu EigeneDatenbank.db
                btnWeiter.Click += (s, e) =>
                {
                    LoadOverlay.Visibility = Visibility.Visible;
                    Task.Run(() =>
                    {
                        Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                ImportIntoDB importIntoDb = new ImportIntoDB(_imodel, $"{_dateienPath}/EigeneDatenbank.db");
                                _model.Navigate(new Import3("Daten erfolgreich importiert", true));
                                return;
                            }
                            catch (Exception ex)
                            {
                                _model.Navigate(new Import3(ex.Message, false));
                            }
                        });
                    });
                };

                // 5. Wenn Abbrechen geklickt wird, lösche Temp/Temp.db
                btnCancel.Click += (s, e) =>
                {
                    File.Delete("Temp/Temp.db");
                    _model.Navigate(_model.History.FindLast(x => x.GetType() == typeof(Import2)));
                    return;
                };
            };
        }
    }
}
