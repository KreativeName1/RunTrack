using System.IO;
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
        public ImportUbersicht()
        {
            InitializeComponent();
            _imodel = FindResource("imodel") as ImportModel ?? new();
            _model = FindResource("pmodel") as MainModel ?? new();

            this.Loaded += (s, e) =>
            {
                try
                {
                    // 1. Lösche Temp/Temp.db, falls vorhanden
                    if (File.Exists("Temp/Temp.db")) File.Delete("Temp/Temp.db");

                    // 2. Kopiere EigeneDatenbank.db zu Temp/Temp.db
                    File.Copy("Dateien/EigeneDatenbank.db", "Temp/Temp.db", true);

                    // 3. Import-Daten in die temporäre Datenbank einfügen
                    ImportIntoDB importIntoDB = new(_imodel, "Temp/Temp.db");
                }
                catch (Exception ex)
                {
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
                    dumodel.ConnectionString = "Temp/Temp.db";
                }
                catch (Exception ex)
                {
                    _model?.Navigate(new Import3(ex.Message, false));
                    return;
                }

                // 5. Wenn Bestätigen geklickt wird, verschiebe Temp/Temp.db zu EigeneDatenbank.db
                btnWeiter.Click += (s, e) =>
                {
                    try
                    {
                        ImportIntoDB importIntoDB = new(_imodel, "Dateien/EigeneDatenbank.db");
                        _model.Navigate(new Import3("Daten erfolgreich importiert", true));
                        return;
                    }
                    catch (Exception ex)
                    {
                        _model.Navigate(new Import3(ex.Message, false));
                    }
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
