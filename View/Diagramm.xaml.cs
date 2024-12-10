using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace RunTrack
{
    // Definiert eine Klasse für Diagrammwerte
    public class DiagrammWert
    {
        public string? name; // Name des Wertes
        public int wert; // Wert
        public string? zeit; // Zeit als String
        public Brush? Farbe; // Farbe des Wertes
    }

    // Definiert die Hauptklasse für das Diagrammfenster, die von MetroWindow erbt
    public partial class Diagramm : MetroWindow
    {
        private AuswertungModel _amodel; // Instanz des Auswertungsmodells
        private List<DiagrammWert> diagrammliste = new(); // Liste der Diagrammwerte

        // Konstruktor, der das Auswertungsmodell entgegennimmt
        public Diagramm(AuswertungModel model)
        {
            InitializeComponent();
            _amodel = model;

            // Schließt das Fenster bei Klick auf den Schließen-Button
            btnClose.Click += (s, e) => Close();

            // Iteriert über die Liste im Auswertungsmodell
            foreach (object item in _amodel.Liste)
            {
                string name, bewertung, bewertung2 = "";
                int bewertungzr = 0;

                // Extrahiert den Namen und die Bewertung aus dem aktuellen Element
                name = (string?)item.GetType()?.GetProperty("Name")?.GetValue(item, null) ?? string.Empty;
                bewertung = (string?)item.GetType()?.GetProperty("Bewertung")?.GetValue(item, null) ?? string.Empty;

                // Unterscheidet die Auswertungsart und verarbeitet die Bewertung entsprechend
                if (_amodel.IsDistanz)
                {
                    label.Content = "Diagramm nach Distanz";
                    foreach (char character in bewertung) if (character != '.' && character != ' ' && character != 'm') bewertung2 += character;
                    try { bewertungzr = Convert.ToInt32(bewertung2); }
                    catch (Exception) { }
                }
                else if (_amodel.IsAnzahl)
                {
                    label.Content = "Diagramm nach RundenAnzahl";
                    string[] split = bewertung.Split(' ');

                    bewertungzr = Convert.ToInt32(split[0].Trim());
                }
                else if (_amodel.IsZeit)
                {
                    label.Content = "Diagramm nach Schnellster Runde";
                    string[] split = bewertung.Split(':');
                    split[0] = split[0].Trim();
                    split[1] = split[1].Trim();
                    bewertungzr += (Convert.ToInt32(split[0])) * 60;
                    bewertungzr += Convert.ToInt32(split[1]);
                }

                // Erstellt ein neues DiagrammWert-Objekt und fügt es der Liste hinzu
                DiagrammWert d = new()
                {
                    name = name,
                    wert = bewertungzr,
                    zeit = bewertung
                };

                // Generiert eine zufällige Farbe für den Wert
                Random r = new();
                d.Farbe = new SolidColorBrush(Color.FromRgb((byte)r.Next(150, 255), (byte)r.Next(150, 255), (byte)r.Next(150, 255)));
                diagrammliste.Add(d);
            }

            // Zeichnet das Diagramm
            try { drawCanvas(); } catch (Exception) { }
        }

        // Formatiert den Wert je nach Auswertungsart
        private string FormatWert(int wert)
        {
            if (_amodel.IsZeit)
            {
                double minutes = wert / 60;
                double seconds = wert % 60;
                if (minutes <= 1) return $"{minutes:0}:{seconds:00} Minute";
                else return $"{minutes:0}:{seconds:00} Minuten";
            }
            else if (_amodel.IsDistanz) return $"{wert} m";
            else
            {
                if (wert <= 1) return $"{wert} Runde";
                else return $"{wert} Runden";
            }
        }

        // Formatiert den Durchschnittswert je nach Auswertungsart
        private string FormatWertDurchschnitt(int wert)
        {
            if (_amodel.IsZeit)
            {
                double minutes = wert / 60;
                double seconds = wert % 60;
                return $"Ø {minutes:0}:{seconds:00} min";
            }
            else if (_amodel.IsDistanz) return $"Ø {wert} m";
            else return $"Ø {wert}";
        }

        // Formatiert den Wert mit einer kurzen Einheit
        private string FormatWertWithShortUnit(int wert)
        {
            if (_amodel.IsZeit)
            {
                double minutes = wert / 60;
                double seconds = wert % 60;
                return $"{minutes:0}:{seconds:00} min";
            }
            else if (_amodel.IsDistanz) return $"{wert} m";
            else return $"{wert}";
        }

        // Zeichnet das Diagramm auf der Canvas
        private void drawCanvas()
        {
            Diagrammcanvas.Children.Clear();
            canvascanvas.Children.Clear();
            double maxWert = 0;
            double durchschnitt = 0;
            double balkenAbstand = 5;

            // Bestimmt den maximalen Wert und den Durchschnittswert
            foreach (DiagrammWert dgwert in diagrammliste)
            {
                if (dgwert.wert > maxWert) maxWert = dgwert.wert;
                durchschnitt += dgwert.wert;
            }

            durchschnitt = Math.Round(durchschnitt / diagrammliste.Count, 3);

            // Zeichnet die Seitendiagramme
            try
            {
                Seitendiagramm_Unten(maxWert);
                Seitendiagramm_Linie(durchschnitt, maxWert);
            }
            catch (Exception)
            {
            }

            // Berechnet die Balkenhöhe und -breite und fügt die Balken zur Canvas hinzu
            if (Diagrammcanvas.ActualHeight > 1)
            {
                double pixelprozahl = Math.Round((Diagrammcanvas.ActualHeight - 24) / maxWert, 3);
                double canvasHeight = Diagrammcanvas.ActualHeight;
                double canvasWidth = Diagrammcanvas.ActualWidth;
                double balkenBreite = (canvasWidth - balkenAbstand) / diagrammliste.Count;
                balkenBreite = balkenBreite - balkenAbstand;

                for (int i = 0; i < diagrammliste.Count; i++)
                {
                    DiagrammWert d = diagrammliste[i];
                    double balkenHoehe = Math.Round(d.wert * pixelprozahl, 3);

                    // Fügt ein Label über dem Balken hinzu, wenn die Höhe des Balkens klein ist
                    if (balkenHoehe < 22)
                    {
                        Label aboveLabel = new()
                        {
                            Content = d.name,
                            Width = balkenBreite,
                            Height = 24,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Tag = d
                        };

                        Canvas.SetLeft(aboveLabel, i * (balkenBreite + balkenAbstand) + balkenAbstand);
                        Canvas.SetBottom(aboveLabel, balkenHoehe + 5);
                        Diagrammcanvas.Children.Add(aboveLabel);
                    }

                    // Erstellt einen Balken und fügt ihn zur Canvas hinzu
                    Border border = new()
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Child = new Label()
                        {
                            Background = d.Farbe,
                            Content = d.name,
                            Width = balkenBreite,
                            Height = balkenHoehe,
                            ToolTip = $"{d.name} │ {FormatWert(d.wert)}",
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Tag = d,
                            Cursor = Cursors.Hand
                        }
                    };

                    Canvas.SetLeft(border, i * (balkenBreite + balkenAbstand) + balkenAbstand);
                    Canvas.SetBottom(border, 0);

                    Diagrammcanvas.Children.Add(border);
                }
            }
        }

        // Event-Handler für Größenänderungen des Grids
        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            try { drawCanvas(); }
            catch (Exception) { }
        }

        // Zeichnet die Durchschnittslinie im Seitendiagramm
        private void Seitendiagramm_Linie(double durchschnitt, double maxwert)
        {
            var lineColor = Color.FromRgb(50, 50, 50);
            var textColor = Color.FromRgb(17, 66, 50);
            if (durchschnitt == maxwert)
            {
                Rectangle obereHorizontaleLinie = new()
                {
                    Width = 3,
                    Fill = new SolidColorBrush(lineColor)
                };

                obereHorizontaleLinie.Height = canvascanvas.ActualHeight - 48;

                Canvas.SetLeft(obereHorizontaleLinie, canvascanvas.ActualWidth / 2 - 1);
                Canvas.SetTop(obereHorizontaleLinie, 24);
                canvascanvas.Children.Add(obereHorizontaleLinie);
            }
            else
            {
                if (canvascanvas.ActualHeight >= 210)
                {
                    Rectangle Durchschnittslinie = new()
                    {
                        Height = 2,
                        Width = canvascanvas.ActualWidth - 10,
                        Fill = new SolidColorBrush(lineColor)
                    };

                    double wertpp = Math.Round((canvascanvas.ActualHeight - 24) / maxwert, 2);

                    Label Durchschnittsbeschriftung = new()
                    {
                        Content = FormatWertDurchschnitt((int)durchschnitt),
                        Width = canvascanvas.ActualWidth,
                        Height = 24,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.FromRgb(46, 136, 182)),
                        FontWeight = FontWeights.Bold
                    };

                    double centerLabelBottom = wertpp * durchschnitt + 1;
                    double aboveLabelTop = 24 + 30;

                    if (canvascanvas.ActualHeight - centerLabelBottom < aboveLabelTop)
                    {
                        centerLabelBottom = canvascanvas.ActualHeight - aboveLabelTop;
                    }

                    Rectangle untereHorizontaleLinie = new()
                    {
                        Height = wertpp * durchschnitt - 20,
                        Width = 3,
                        Fill = new SolidColorBrush(lineColor)
                    };

                    Rectangle obereHorizontaleLinie = new()
                    {
                        Height = wertpp * (maxwert - durchschnitt) - 24,
                        Width = 3,
                        Fill = new SolidColorBrush(lineColor)
                    };

                    if (obereHorizontaleLinie.Height > 10)
                    {
                        Canvas.SetLeft(Durchschnittslinie, 5);
                        Canvas.SetBottom(Durchschnittslinie, wertpp * durchschnitt);
                        canvascanvas.Children.Add(Durchschnittslinie);

                        Canvas.SetLeft(Durchschnittsbeschriftung, 0);
                        Canvas.SetBottom(Durchschnittsbeschriftung, centerLabelBottom);
                        canvascanvas.Children.Add(Durchschnittsbeschriftung);

                        Canvas.SetLeft(untereHorizontaleLinie, canvascanvas.ActualWidth / 2 - 1);
                        Canvas.SetBottom(untereHorizontaleLinie, 20);
                        canvascanvas.Children.Add(untereHorizontaleLinie);

                        Canvas.SetLeft(obereHorizontaleLinie, canvascanvas.ActualWidth / 2 - 1);
                        Canvas.SetTop(obereHorizontaleLinie, 24);
                        canvascanvas.Children.Add(obereHorizontaleLinie);
                    }
                    else
                    {
                        obereHorizontaleLinie.Height = canvascanvas.ActualHeight - 48;

                        Canvas.SetLeft(obereHorizontaleLinie, canvascanvas.ActualWidth / 2 - 1);
                        Canvas.SetTop(obereHorizontaleLinie, 24);
                        canvascanvas.Children.Add(obereHorizontaleLinie);
                    }
                }
                else
                {
                    Rectangle obereHorizontaleLinie = new()
                    {
                        Width = 3,
                        Fill = new SolidColorBrush(lineColor)
                    };

                    obereHorizontaleLinie.Height = canvascanvas.ActualHeight - 48;

                    Canvas.SetLeft(obereHorizontaleLinie, canvascanvas.ActualWidth / 2 - 1);
                    Canvas.SetTop(obereHorizontaleLinie, 24);
                    canvascanvas.Children.Add(obereHorizontaleLinie);
                }
            }
        }

        // Zeichnet die untere Linie im Seitendiagramm
        private void Seitendiagramm_Unten(double maxwert)
        {
            var lineColor = Color.FromRgb(50, 50, 50);
            var textColor = Color.FromRgb(0, 0, 0);

            Rectangle bottomLine = new()
            {
                Height = 2,
                Width = canvascanvas.ActualWidth - 10,
                Fill = new SolidColorBrush(lineColor)
            };

            Canvas.SetLeft(bottomLine, 5);
            Canvas.SetBottom(bottomLine, 0);
            canvascanvas.Children.Add(bottomLine);

            Label belowLabel = new()
            {
                Content = FormatWertWithShortUnit(0),
                Width = canvascanvas.ActualWidth,
                Height = 24,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(46, 136, 182)),
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(belowLabel, 0);
            Canvas.SetBottom(belowLabel, bottomLine.Height - 3);
            canvascanvas.Children.Add(belowLabel);

            Rectangle topLine = new()
            {
                Height = 2,
                Width = canvascanvas.ActualWidth - 10,
                Fill = new SolidColorBrush(lineColor)
            };

            Canvas.SetLeft(topLine, 5);
            Canvas.SetTop(topLine, 24);

            Label aboveLabel = new()
            {
                Content = FormatWertWithShortUnit((int)maxwert),
                Width = canvascanvas.ActualWidth,
                Height = 24,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(46, 136, 182)),
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(aboveLabel, 0);
            Canvas.SetTop(aboveLabel, 4);
            canvascanvas.Children.Add(aboveLabel);
            canvascanvas.Children.Add(topLine);
        }

        // Event-Handler für den Klick auf den Schließen-Button
        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event-Handler für das MouseEnter-Ereignis des Schließen-Buttons
        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }

        // Event-Handler für das MouseLeave-Ereignis des Schließen-Buttons
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }
    }
}
