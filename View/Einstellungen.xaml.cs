using RunTrack.View;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Einstellungen.xaml
    /// </summary>
    public partial class Einstellungen : Page
    {
        private MainViewModel _mvmodel;
        private PageModel _pmodel;
        private bool _changesMade;

        public Einstellungen()
        {
            InitializeComponent();

            _mvmodel = FindResource("mvmodel") as MainViewModel ?? new MainViewModel();
            _pmodel = FindResource("pmodel") as PageModel ?? new PageModel();

            DataContext = this;

            LoadContent();
        }

        private void LoadContent()
        {
            using (var db = new LaufDBContext())
            {
                var sortedRundenArten = db.RundenArten.OrderBy(r => r.MaxScanIntervalInSekunden).ToList();
                int rowIndex = 0;

                foreach (RundenArt rundenArt in sortedRundenArten)
                {
                    // Zeilen hinzufügen
                    GridSettings.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

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

                    // Löschen
                    Button deleteButton = new()
                    {
                        Content = "🗑️",
                        FontSize = 12,
                        Width = 25,
                        Height = 25,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(30, 1.5, 5, 2.5),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                        ToolTip = "Löschen",
                        Tag = rundenArt.Name
                    };
                    deleteButton.Click += DeleteButton_Click;

                    Grid.SetRow(deleteButton, rowIndex);
                    Grid.SetColumn(deleteButton, 4);
                    GridSettings.Children.Add(deleteButton);

                    // Einstellungen
                    Button optionsButton = new()
                    {
                        Content = "⚙️",
                        FontSize = 12,
                        Width = 25,
                        Height = 25,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(0, 1.5, 10, 2.5),
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4")),
                        ToolTip = "Einstellungen",
                        Tag = rundenArt.Name
                    };
                    optionsButton.Click += OptionsButton_Click;

                    Grid.SetRow(optionsButton, rowIndex);
                    Grid.SetColumn(optionsButton, 5);
                    GridSettings.Children.Add(optionsButton);

                    rowIndex++;
                }

                GridSettings.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Add Rectangle
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

                Button addButton = new()
                {
                    Content = "➕",
                    FontSize = 12,
                    Width = 25,
                    Height = 25,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0),
                    ToolTip = "Neue Rundenart",
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F8F4"))
                };
                addButton.Click += AddButton_Click;

                Grid.SetRow(addButton, rowIndex);
                Grid.SetColumnSpan(addButton, 6);
                GridSettings.Children.Add(addButton);
            }
        }


        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _changesMade = true;
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
                        RefreshGridSettings();
                    }
                    else
                    {
                        new Popup().Display("Fehler", $"Rundenart '{rundenartName}' nicht gefunden", PopupType.Error, PopupButtons.Ok);
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
            // if (System.Windows.MessageBox.Show("Wollen Sie wirklich " + rundenartName + " löschen?", "Löschne", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            if (new Popup().Display("Löschen", "Wollen Sie wirklich " + rundenartName + " löschen?", PopupType.Question, PopupButtons.YesNo).Result == true)
            {
                using (var db = new LaufDBContext())
                {
                    var rundenart = db.RundenArten.FirstOrDefault(r => r.Name == rundenartName);
                    if (rundenart != null)
                    {
                        db.RundenArten.Remove(rundenart);
                        db.SaveChanges();
                    }
                }
            }

            RefreshGridSettings();
        }

        private void RefreshGridSettings()
        {
            GridSettings.Children.Clear();
            GridSettings.RowDefinitions.Clear();

            LoadContent();
            _changesMade = false;
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

                //System.Windows.MessageBox.Show("Einstellungen wurden gespeichert", "Einstellungen gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
                new Popup().Display("Einstellungen gespeichert", "Einstellungen wurden gespeichert", PopupType.Info, PopupButtons.Ok);
            }
            else
            {
                //System.Windows.MessageBox.Show("Keine Änderungen zum Speichern gefunden", "Keine Änderungen", MessageBoxButton.OK, MessageBoxImage.Information);
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
            btnSave.Visibility = Visibility.Visible;
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
            // DialogMode + Zusatz noch hinzufügen wie bei AddButton_Click
            AdminEinstellungen adminEinstellungen = new(DialogMode.Neu);
            _pmodel.Navigate(adminEinstellungen);
        }

        private void btnPasswordChange_Click(object sender, RoutedEventArgs e)
        {
            // DialogMode + Zusatz noch hinzufügen wie bei OptionsButton_Click
            AdminEinstellungen adminEinstellungen = new(DialogMode.Bearbeiten, this._mvmodel.Benutzer.Vorname, this._mvmodel.Benutzer.Nachname);
            _pmodel.Navigate(adminEinstellungen);
        }
    }
}
