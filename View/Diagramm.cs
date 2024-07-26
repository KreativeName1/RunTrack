using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace Klimalauf
{
    public class DiagrammWert
    {
        public string? name;
        public int wert;
        public string? zeit;
        public Brush? Farbe;
    }

    public partial class Diagramm : Window
    {
        private AuswertungModel _amodel;
        private List<DiagrammWert> diagrammliste = new List<DiagrammWert>();

        public Diagramm()
        {
            string Diagrammtitel = "";
            InitializeComponent();
            _amodel = FindResource("amodel") as AuswertungModel ?? new();

            btnClose.Click += (s, e) => Close();

            foreach (object item in _amodel.Liste)
            {
                string name, bewertung, bewertung2 = "";

                int bewertungzr = 0;
                name = (string?)item.GetType()?.GetProperty("Name")?.GetValue(item, null) ?? string.Empty;
                bewertung = (string?)item.GetType()?.GetProperty("Bewertung")?.GetValue(item, null) ?? string.Empty;

                if (_amodel.IsDistanz)
                {
                    Diagrammtitel = "Diagramm nach Distanz";
                    foreach (char a in bewertung)
                    {
                        if (a != '.' && a != ' ' && a != 'm')
                        {
                            bewertung2 += a;
                        }
                    }
                    try
                    {
                        bewertungzr = Convert.ToInt32(bewertung2);
                    }
                    catch (Exception)
                    {
                        // Handle the exception if needed
                    }
                }
                else if (_amodel.IsAnzahl)
                {
                    Diagrammtitel = "Diagramm nach RundenAnzahl";
                    bewertungzr = Convert.ToInt32(bewertung);
                }
                else if (_amodel.IsZeit)
                {
                    Diagrammtitel = "Diagramm nach Schnellster Runde";
                    string[] split = bewertung.Split(':');
                    split[0] = split[0].Trim();
                    split[1] = split[1].Trim();
                    bewertungzr += (Convert.ToInt32(split[0])) * 60;
                    bewertungzr += Convert.ToInt32(split[1]);
                }

                DiagrammWert d = new DiagrammWert
                {
                    name = name,
                    wert = bewertungzr,
                    zeit = bewertung
                };

                Random r = new Random();
                d.Farbe = new SolidColorBrush(Color.FromRgb((byte)r.Next(100, 255), (byte)r.Next(100, 255), (byte)r.Next(100, 255)));
                diagrammliste.Add(d);
            }

            label.Content = Diagrammtitel;

            try
            {
                drawCanvas();
            }
            catch (Exception)
            {
                // Handle the exception if needed
            }
        }

        private string FormatWert(int wert)
        {
            if (_amodel.IsZeit)
            {
                // Zeit im Format Minuten:Sekunden
                double minutes = wert / 60;
                double seconds = wert % 60;
                if (minutes <= 1)
                {
                    return $"{minutes:0}:{seconds:00} Minute";
                }
                else
                {
                    return $"{minutes:0}:{seconds:00} Minuten";
                }
            }
            else if (_amodel.IsDistanz)
            {
                // Distanz in Metern
                return $"{wert} m";
            }
            else
            {
                if (wert <= 1)
                {
                    return $"{wert} Runde";
                }
                else
                {
                    return $"{wert} Runden";
                }
            }
        }

        private string FormatWertDurchschnitt(int wert)
        {
            if (_amodel.IsZeit)
            {
                // Zeit im Format Minuten:Sekunden
                double minutes = wert / 60;
                double seconds = wert % 60;

                return $"Ø {minutes:0}:{seconds:00} min";

            }
            else if (_amodel.IsDistanz)
            {
                // Distanz in Metern
                return $"Ø {wert} m";
            }
            else
            {
                return $"Ø {wert}";

            }
        }

        private string FormatWertWithShortUnit(int wert)
        {
            if (_amodel.IsZeit)
            {
                // Zeit im Format Minuten:Sekunden
                double minutes = wert / 60;
                double seconds = wert % 60;
                return $"{minutes:0}:{seconds:00} min";
            }
            else if (_amodel.IsDistanz)
            {
                // Distanz in Metern
                return $"{wert} m";
            }
            else
            {
                // Anzahl oder andere Daten
                return $"{wert}";
            }
        }

        private void drawCanvas()
        {
            Diagrammcanvas.Children.Clear();
            canvascanvas.Children.Clear();
            double maxWert = 0;
            double durchschnitt = 0;
            double balkenAbstand = 5;

            foreach (DiagrammWert dgwert in diagrammliste)
            {
                if (dgwert.wert > maxWert)
                {
                    maxWert = dgwert.wert;
                }

                durchschnitt += dgwert.wert;
            }

            durchschnitt = Math.Round(durchschnitt / diagrammliste.Count, 3); // Round to 3 decimal places

            try
            {
                Seitendiagramm_Unten(maxWert);
                Seitendiagramm_Linie(durchschnitt, maxWert);
            }
            catch (Exception)
            {
            }

            if (Diagrammcanvas.ActualHeight > 1)
            {
                double pixelprozahl = Math.Round((Diagrammcanvas.ActualHeight - 24) / maxWert, 3); // Round to 3 decimal places

                double canvasHeight = Diagrammcanvas.ActualHeight;
                double canvasWidth = Diagrammcanvas.ActualWidth;

                double balkenBreite = (canvasWidth - balkenAbstand) / diagrammliste.Count;
                balkenBreite = balkenBreite - balkenAbstand;

                for (int i = 0; i < diagrammliste.Count; i++)
                {
                    DiagrammWert d = diagrammliste[i];
                    double balkenHoehe = Math.Round(d.wert * pixelprozahl, 3); // Round to 3 decimal places

                    if (balkenHoehe < 22)
                    {
                        Label aboveLabel = new Label
                        {
                            Content = d.name,
                            Width = balkenBreite,
                            Height = 24, // Set a fixed height for the label
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Tag = d // Set the data point as tag
                        };

                        Canvas.SetLeft(aboveLabel, i * (balkenBreite + balkenAbstand) + balkenAbstand);
                        Canvas.SetBottom(aboveLabel, balkenHoehe + 5); // Position the label above the bar
                        Diagrammcanvas.Children.Add(aboveLabel);
                    }

                    Label label = new Label()
                    {
                        Background = d.Farbe,
                        Content = d.name,
                        Width = balkenBreite,
                        Height = balkenHoehe,
                        ToolTip = $"{d.name} │ {FormatWert(d.wert)}", // Set the formatted tooltip
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Tag = d, // Set the data point as tag
                        Cursor = Cursors.Hand
                    };

                    Canvas.SetLeft(label, i * (balkenBreite + balkenAbstand) + balkenAbstand);
                    Canvas.SetBottom(label, 0);

                    Diagrammcanvas.Children.Add(label);
                }
            }
        }

        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            try
            {
                drawCanvas();
            }
            catch (Exception)
            {
                // Handle the exception if needed
            }
        }

        private void Seitendiagramm_Linie(double durchschnitt, double maxwert)
        {
            var lineColor = Color.FromRgb(50, 50, 50);  // Darker gray color for better visibility
            var textColor = Color.FromRgb(17, 66, 50);  // Black color for text
            if (durchschnitt == maxwert)
            {
                Rectangle obereHorizontaleLinie = new Rectangle
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
                    Rectangle Durchschnittslinie = new Rectangle
                    {
                        Height = 2, // Increase the thickness of the line
                        Width = canvascanvas.ActualWidth - 10,
                        Fill = new SolidColorBrush(lineColor)
                    };

                    double wertpp = Math.Round((canvascanvas.ActualHeight - 24) / maxwert, 2); // Round to 2 decimal places

                    Label Durchschnittsbeschriftung = new Label
                    {
                        Content = FormatWertDurchschnitt((int)durchschnitt), // Use FormatWert method
                        Width = canvascanvas.ActualWidth,
                        Height = 24, // Set a fixed height for the label
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.FromRgb(46, 136, 182)), // Set text color
                        FontWeight = FontWeights.Bold // Make the text bold
                    };

                    double centerLabelBottom = wertpp * durchschnitt + 1;
                    double aboveLabelTop = 24 + 30; // 24 for the height of the aboveLabel and 30 pixels margin

                    // Adjust position if the centerLabel is too close to the aboveLabel
                    if (canvascanvas.ActualHeight - centerLabelBottom < aboveLabelTop)
                    {
                        centerLabelBottom = canvascanvas.ActualHeight - aboveLabelTop;
                    }

                    Rectangle untereHorizontaleLinie = new Rectangle
                    {
                        Height = wertpp * durchschnitt - 20,
                        Width = 3, // Increase the thickness of the line
                        Fill = new SolidColorBrush(lineColor)
                    };

                    Rectangle obereHorizontaleLinie = new Rectangle
                    {
                        Height = wertpp * (maxwert - durchschnitt) - 24,
                        Width = 3, // Increase the thickness of the line
                        Fill = new SolidColorBrush(lineColor)
                    };

                    if (obereHorizontaleLinie.Height > 10)
                    {
                        Canvas.SetLeft(Durchschnittslinie, 5);
                        Canvas.SetBottom(Durchschnittslinie, wertpp * durchschnitt);
                        canvascanvas.Children.Add(Durchschnittslinie);

                        Canvas.SetLeft(Durchschnittsbeschriftung, 0);
                        Canvas.SetBottom(Durchschnittsbeschriftung, centerLabelBottom); // Position the label above the line
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
                    Rectangle obereHorizontaleLinie = new Rectangle
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



        private void Seitendiagramm_Unten(double maxwert)
        {
            var lineColor = Color.FromRgb(50, 50, 50);  // Darker gray color for better visibility
            var textColor = Color.FromRgb(0, 0, 0);    // Black color for text

            Rectangle bottomLine = new Rectangle
            {
                Height = 2, // Increase the thickness of the line
                Width = canvascanvas.ActualWidth - 10,
                Fill = new SolidColorBrush(lineColor)
            };

            Canvas.SetLeft(bottomLine, 5);
            Canvas.SetBottom(bottomLine, 0);
            canvascanvas.Children.Add(bottomLine);

            Label belowLabel = new Label
            {
                Content = FormatWertWithShortUnit(0), // Use FormatWert method
                Width = canvascanvas.ActualWidth,
                Height = 24, // Set a fixed height for the label
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(46, 136, 182)), // Set text color
                FontWeight = FontWeights.Bold // Make the text bold
            };

            Canvas.SetLeft(belowLabel, 0);
            Canvas.SetBottom(belowLabel, bottomLine.Height - 3); // Position the label above the bar
            canvascanvas.Children.Add(belowLabel);

            Rectangle topLine = new Rectangle
            {
                Height = 2, // Increase the thickness of the line
                Width = canvascanvas.ActualWidth - 10,
                Fill = new SolidColorBrush(lineColor)
            };

            Canvas.SetLeft(topLine, 5);
            Canvas.SetTop(topLine, 24);

            Label aboveLabel = new Label
            {
                Content = FormatWertWithShortUnit((int)maxwert), // Use FormatWert method
                Width = canvascanvas.ActualWidth,
                Height = 24, // Set a fixed height for the label
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(46, 136, 182)), // Set text color
                FontWeight = FontWeights.Bold // Make the text bold
            };

            Canvas.SetLeft(aboveLabel, 0);
            Canvas.SetTop(aboveLabel, 4); // Position the label above the bar
            canvascanvas.Children.Add(aboveLabel);
            canvascanvas.Children.Add(topLine);
        }
    }
}
