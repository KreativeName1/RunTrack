using RunTrack.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Einstellungen.xaml
    /// </summary>
    public partial class Einstellungen : Page
    {
        private MainModel _pmodel; // Hauptmodell
        private bool _changesMade; // Flag für Änderungen

        public Einstellungen()
        {
            InitializeComponent();

            // Initialisiert das Hauptmodell
            _pmodel = FindResource("pmodel") as MainModel ?? new MainModel();

            DataContext = this;

            // Lädt den Inhalt und aktualisiert die Sichtbarkeit des Speichern-Buttons
            LoadContent();
            UpdateSaveButtonVisibility();
        }

        private void LoadContent()
        {
            // Setzt den Text des Schlüssels
            tbKey.Text = UniqueKey.GetKey();

            using (var db = new LaufDBContext())
            {
                // Sortiert die Rundenarten und fügt sie dem Grid hinzu
                var sortedRundenArten = db.RundenArten.OrderBy(r => r.MaxScanIntervalInSekunden).ToList();
                int rowIndex = 0;

                foreach (RundenArt rundenArt in sortedRundenArten)
                {
                    // Zeilen hinzufügen
                    GridSettings.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    // Erstellt und fügt ein Label hinzu
                    Label label = new()
                    {
                        Content = rundenArt.Name,
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 1.5, 0, 2.5)
                    };
                    Grid.SetRow(label, rowIndex);
                    Grid.SetColumn(label, 0);
                    GridSettings.Children.Add(label);

                    // Erstellt und fügt ein Spacer-Label hinzu
                    Label labelSpacer = new()
                    {
                        Content = "→",
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 1.5, 8, 2.5)
                    };
                    Grid.SetRow(labelSpacer, rowIndex);
                    Grid.SetColumn(labelSpacer, 1);
                    GridSettings.Children.Add(labelSpacer);

                    // Erstellt und fügt ein IntegerUpDown-Steuerelement hinzu
                    IntegerUpDown integerUpDown = new()
                    {
                        Name = rundenArt.Name.Replace(" ", "_"),
                        Value = rundenArt.MaxScanIntervalInSekunden,
                        Width = 50,
                        Height = 20,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.Blue)
                    };
                    integerUpDown.ValueChanged += IntegerUpDown_ValueChanged;

                    Grid.SetRow(integerUpDown, rowIndex);
                    Grid.SetColumn(integerUpDown, 2);
                    GridSettings.Children.Add(integerUpDown);

                    // Erstellt und fügt ein Label für Sekunden hinzu
                    Label labelSeconds = new()
                    {
                        Content = "Sekunde/n",
                        FontSize = 14,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 1.5, 0, 2.5)
                    };
                    Grid.SetRow(labelSeconds, rowIndex);
                    Grid.SetColumn(labelSeconds, 3);
                    GridSettings.Children.Add(labelSeconds);

                    // Erstellt und fügt einen Löschen-Button hinzu
                    Button deleteButton = new()
                    {
                        Content = "🗑️",
                        FontSize = 12,
                        Width = 35,
                        Height = 35,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(30, 1.5, 5, 2.5),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                        ToolTip = "Löschen",
                        Tag = rundenArt.Name,
                        Cursor = Cursors.Hand
                    };
                    Image buttonImageDelete = new()
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Images/buttonIcons/delete.png")),
                        Width = 20,
                        Height = 20
                    };
                    deleteButton.Content = buttonImageDelete;
                    deleteButton.Click += DeleteButton_Click;

                    Grid.SetRow(deleteButton, rowIndex);
                    Grid.SetColumn(deleteButton, 4);
                    GridSettings.Children.Add(deleteButton);

                    // Erstellt und fügt einen Einstellungen-Button hinzu
                    Button optionsButton = new()
                    {
                        Content = "⚙️",
                        FontSize = 12,
                        Width = 35,
                        Height = 35,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 1.5, 10, 2.5),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                        ToolTip = "Einstellungen",
                        Tag = rundenArt.Name,
                        Cursor = Cursors.Hand
                    };
                    Image buttonImageOptions = new()
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Images/buttonIcons/settings.png")),
                        Width = 20,
                        Height = 20
                    };
                    optionsButton.Content = buttonImageOptions;
                    optionsButton.Click += OptionsButton_Click;

                    Grid.SetRow(optionsButton, rowIndex);
                    Grid.SetColumn(optionsButton, 5);
                    GridSettings.Children.Add(optionsButton);

                    rowIndex++;
                }

                GridSettings.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Fügt ein Trennrechteck hinzu
                Rectangle rect = new()
                {
                    Height = 1,
                    Fill = new SolidColorBrush(Colors.Gray),
                    Margin = new Thickness(0, 5, 0, 30)
                };

                Grid.SetRow(rect, rowIndex);
                Grid.SetColumnSpan(rect, 6);
                GridSettings.Children.Add(rect);

                rowIndex++;

                // Fügt einen Hinzufügen-Button hinzu
                Button addButton = new()
                {
                    Content = "➕",
                    FontSize = 12,
                    Width = 35,
                    Height = 35,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                    ToolTip = "Neue Rundenart",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                    Cursor = Cursors.Hand
                };
                addButton.Click += AddButton_Click;

                Grid.SetRow(addButton, rowIndex);
                Grid.SetColumnSpan(addButton, 6);
                GridSettings.Children.Add(addButton);
            }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _changesMade = true; // Setzt das Flag für Änderungen
            UpdateSaveButtonVisibility(); // Aktualisiert die Sichtbarkeit des Speichern-Buttons
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LaufDBContext())
            {
                if (db.RundenArten.Count() < 6)
                {
                    VerwaltungRunden verwaltungRunden = new(DialogMode.Neu);
                    _pmodel.Navigate(verwaltungRunden);
                    RefreshGridSettings();
                }
                else
                {
                    new Popup().Display("Limit erreicht", "Sie haben das Limit von 6 Rundenarten erreicht!", PopupType.Warning, PopupButtons.Ok);
                }
            }
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Button? optionsButton = sender as Button;
            string rundenartName = (string?)optionsButton?.Tag ?? string.Empty;

            if (!string.IsNullOrEmpty(rundenartName))
            {
                using (var db = new LaufDBContext())
                {
                    var rundenArt = db.RundenArten.FirstOrDefault(r => r.Name == rundenartName);
                    if (rundenArt != null)
                    {
                        VerwaltungRunden verwaltungRunden = new(DialogMode.Bearbeiten, rundenArt);
                        _pmodel.Navigate(verwaltungRunden);
                    }
                    else
                    {
                        new Popup().Display("Fehler", $"Rundenart '{rundenartName.ToUpper()}' nicht gefunden", PopupType.Error, PopupButtons.Ok);
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button? deleteButton = sender as Button;
            if (deleteButton != null)
            {
                string? rundenartName = deleteButton.Tag as string;
                if (!string.IsNullOrEmpty(rundenartName))
                {
                    DeleteRundenart(rundenartName);
                }
            }
        }

        private void DeleteRundenart(string rundenartName)
        {
            if (new Popup().Display("Löschen", "Wollen Sie wirklich " + rundenartName.ToUpper() + " löschen?", PopupType.Question, PopupButtons.YesNo) == true)
            {
                using (var db = new LaufDBContext())
                {
                    var rundenart = db.RundenArten.FirstOrDefault(r => r.Name == rundenartName);

                    if (rundenart != null)
                    {
                        var klasse = db.Klassen.FirstOrDefault(k => k.RundenArtId == rundenart.Id);
                        var laeufer = db.Laeufer.FirstOrDefault(l => l.RundenArtId == rundenart.Id);
                        if (klasse != null || laeufer != null)
                        {
                            new Popup().Display("Fehler", $"Rundenart '{rundenartName.ToUpper()}' kann nicht gelöscht werden, da sie in einer Klasse oder bei einem Läufer verwendet wird.", PopupType.Error, PopupButtons.Ok);
                            return;
                        }
                        db.RundenArten.Remove(rundenart);
                        db.SaveChanges();
                    }
                }
            }

            RefreshGridSettings();
        }

        public void RefreshGridSettings()
        {
            GridSettings.Children.Clear();
            GridSettings.RowDefinitions.Clear();

            LoadContent();
            _changesMade = false;
            UpdateSaveButtonVisibility();
        }

        private void LogoutIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _pmodel.Navigate(new MainWindow());
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_changesMade)
            {
                using (var db = new LaufDBContext())
                {
                    foreach (var item in GridSettings.Children)
                    {
                        if (item is IntegerUpDown integerUpDown)
                        {
                            db.RundenArten.First(r => r.Name.Replace(" ", "_") == integerUpDown.Name).MaxScanIntervalInSekunden = integerUpDown.Value ?? 0;
                        }
                    }
                    db.SaveChanges();
                }

                new Popup().Display("Einstellungen gespeichert", "Einstellungen wurden gespeichert", PopupType.Info, PopupButtons.Ok);
                _changesMade = false;
                UpdateSaveButtonVisibility();
            }
            else
            {
                new Popup().Display("Keine Änderungen", "Keine Änderungen zum Speichern gefunden", PopupType.Info, PopupButtons.Ok);
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            _pmodel.Navigate(new Scanner());
        }

        private void Runden_Click(object sender, RoutedEventArgs e)
        {
            txtOptions.Visibility = Visibility.Collapsed;
            PanelAdminSettings.Visibility = Visibility.Collapsed;
            txtAdminTitel.Visibility = Visibility.Collapsed;
            GridSettings.Visibility = Visibility.Visible;
            txtRoundsTitel.Visibility = Visibility.Visible;
            btnRounds.IsEnabled = false;
            btnAdmin.IsEnabled = true;
            UpdateSaveButtonVisibility();
        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            txtOptions.Visibility = Visibility.Collapsed;
            GridSettings.Visibility = Visibility.Collapsed;
            txtRoundsTitel.Visibility = Visibility.Collapsed;
            PanelAdminSettings.Visibility = Visibility.Visible;
            txtAdminTitel.Visibility = Visibility.Visible;
            btnRounds.IsEnabled = true;
            btnAdmin.IsEnabled = false;
            btnSave.Visibility = Visibility.Hidden;
        }

        private void btnAdminAdd_Click(object sender, RoutedEventArgs e)
        {
            AdminEinstellungen adminEinstellungen = new(DialogMode.Neu);
            _pmodel.Navigate(adminEinstellungen);
        }

        private void btnAdminSum_Click(object sender, RoutedEventArgs e)
        {
            AdminVerwalten adminVerwalten = new();
            _pmodel.Navigate(adminVerwalten);
        }

        private void btnPasswordChange_Click(object sender, RoutedEventArgs e)
        {
            AdminEinstellungen adminEinstellungen = new(DialogMode.Bearbeiten, this._pmodel.Benutzer.Vorname, this._pmodel.Benutzer.Nachname);
            _pmodel.Navigate(adminEinstellungen);
        }

        private void UpdateSaveButtonVisibility()
        {
            btnSave.Visibility = _changesMade ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
