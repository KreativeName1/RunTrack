using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace Klimalauf
{
    /// <summary>
    /// Interaktionslogik für Diagrammm.xaml
    /// </summary>
    public class DiagrammWert
    {
        public string name;
        public int wert;
        public string zeit;
        public Brush Farbe;
    }
    public partial class Diagramm : Window
    {
        private AuswertungModel _amodel;
        private List<DiagrammWert> diagrammliste = new List<DiagrammWert>();

        public Diagramm()
        {
            string Diagrammtitel = "";
            InitializeComponent();
            _amodel = FindResource("amodel") as AuswertungModel;

            btnClose.Click += (s, e) => Close();

            foreach (object item in _amodel.Liste)
            {
                string name, bewertung, bewertung2 = "";

                int bewertungzr = 0;
                name = (string)item.GetType().GetProperty("Name").GetValue(item, null);
                bewertung = (string)item.GetType().GetProperty("Bewertung").GetValue(item, null);
                
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
                    catch (Exception e)
                    {

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
                    string[] split = new string[2];
                    split = bewertung.Split(':');
                    split[0] = split[0].Trim();
                    split[1] = split[1].Trim();
                    bewertungzr += (Convert.ToInt32(split[0])) * 60;
                    bewertungzr += Convert.ToInt32(split[1]);
                }

                DiagrammWert d = new DiagrammWert();
                d.name = name;
                d.wert = bewertungzr;
                d.zeit = (string)item.GetType().GetProperty("Bewertung").GetValue(item, null);
                Random r = new Random();

                d.Farbe = new SolidColorBrush(Color.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255)));
                diagrammliste.Add(d);


            }
            label.Content = Diagrammtitel;

            try
            {
                drawCanvas();
            }
            catch (Exception ex)
            {

            }



        }

        private void drawCanvas()
        {


            Diagrammcanvas.Children.Clear();
            canvascanvas.Children.Clear();
            double maxWert = 0;
            double durchschnitt = 0;
            double balkenAbstand = 7;
            foreach (DiagrammWert dgwert in diagrammliste)
            {
                if (dgwert.wert > maxWert)
                {
                    maxWert = dgwert.wert;
                }

                durchschnitt += dgwert.wert;

            }

            durchschnitt = durchschnitt / diagrammliste.Count;
            try{
                Seitendiagramm_Unten(maxWert);
                Seitendiagramm_Linie(durchschnitt, maxWert);
            }
            catch(Exception ex)
            {

            }
            
            if(Diagrammcanvas.ActualHeight > 1) {
            double pixelprozahl = (Diagrammcanvas.ActualHeight - 24) / maxWert;

            double canvasHeight = Diagrammcanvas.ActualHeight;

            double canvasWidth = Diagrammcanvas.ActualWidth;

            
            double balkenBreite = (canvasWidth - balkenAbstand) / diagrammliste.Count;
            balkenBreite = balkenBreite - balkenAbstand;


            for (int i = 0; i < diagrammliste.Count; i++)
            {
                DiagrammWert d = diagrammliste[i];
                double balkenHoehe = d.wert * pixelprozahl;

                // Überprüfung, ob Balkenhöhe kleiner als 20 ist
                if (balkenHoehe < 22)
                {
                    Label aboveLabel = new Label();
                    aboveLabel.Content = d.name;
                    aboveLabel.Width = balkenBreite;
                    aboveLabel.Height = 24; // Setze eine feste Höhe für das Label
                    aboveLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    aboveLabel.Tag = d; // Setze den Datenpunkt als Tag



                    Canvas.SetLeft(aboveLabel, i * (balkenBreite + balkenAbstand) + balkenAbstand);
                    Canvas.SetBottom(aboveLabel, balkenHoehe + 5); // Positioniere das Label über dem Balken
                    Diagrammcanvas.Children.Add(aboveLabel);
                }

                Label label = new Label()
                {
                    Background = d.Farbe,
                    Content = d.name,
                    Width = balkenBreite,
                    Height = balkenHoehe,
                    ToolTip = d.name + " " + d.zeit,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = d, // Setze den Datenpunkt als Tag
                    Cursor = Cursors.Hand
                    
                };
                    



                Canvas.SetLeft(label, i * (balkenBreite + balkenAbstand) + balkenAbstand);
                Canvas.SetBottom(label, 0);




                Diagrammcanvas.Children.Add(label);
            }
            }

        }
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                drawCanvas();
            }
            catch (Exception ex)
            {

            }

        }

        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            try
            {
                drawCanvas();
            }
            catch (Exception ex)
            {
             
            }
        }

        private void Seitendiagramm_Linie(double durchschnitt, double maxwert)
        {

            Label d = new Label();

            d.Height = 2;
            d.Width = canvascanvas.ActualWidth;
            d.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            double wertpp = (canvascanvas.ActualHeight - 24) / maxwert;


            Canvas.SetLeft(d, 0);
            Canvas.SetBottom(d, wertpp * durchschnitt);


            

            Label aboveLabel = new Label();


            if (_amodel.IsZeit)
            {
                double a = durchschnitt % 60;
                a = Math.Round(a, 0);
                double b = (durchschnitt - a) / 60;
                b = Math.Round(b, 0);
                aboveLabel.Content += "Ø" + b + ":" + a;
            }
            else if (_amodel.IsDistanz)
            {
                aboveLabel.Content = durchschnitt + "m";
            }
            else
            {
                aboveLabel.Content = durchschnitt;
            }



            aboveLabel.Width = d.Width;
            aboveLabel.Height = 24; // Setze eine feste Höhe für das Label
            aboveLabel.HorizontalContentAlignment = HorizontalAlignment.Center;

            Canvas.SetLeft(aboveLabel, 0);
            Canvas.SetBottom(aboveLabel, wertpp * durchschnitt); // Positioniere das Label über dem Balken
            if(durchschnitt < maxwert) { 
            canvascanvas.Children.Add(d);
            canvascanvas.Children.Add(aboveLabel);
            }

            Label untere_horizontale_linie = new Label();

            untere_horizontale_linie.Height = wertpp * durchschnitt - 20;
            untere_horizontale_linie.Width = 2;
            untere_horizontale_linie.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            Canvas.SetLeft(untere_horizontale_linie, canvascanvas.ActualWidth / 2);
            Canvas.SetBottom(untere_horizontale_linie, 20);
            canvascanvas.Children.Add(untere_horizontale_linie);


            Label obere_h_l = new Label();

            obere_h_l.Height = wertpp * (maxwert - durchschnitt) - 24;
            obere_h_l.Width = 2;
            obere_h_l.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            Canvas.SetLeft(obere_h_l, canvascanvas.ActualWidth / 2);
            Canvas.SetTop(obere_h_l, 24);
            canvascanvas.Children.Add(obere_h_l);

        }

        private void Seitendiagramm_Unten(double maxwert)
        {


            Label b = new Label();

            b.Height = 2;
            b.Width = canvascanvas.ActualWidth;
            b.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            Canvas.SetLeft(b, 0);
            Canvas.SetBottom(b, 0);
            canvascanvas.Children.Add(b);


            Label aboveLabel_unten = new Label();


            if (_amodel.IsZeit)
            {
                double a = maxwert % 60;
                a = Math.Round(a, 0);
                double b2 = (maxwert - a) / 60;
                b2 = Math.Round(b2, 0);
                aboveLabel_unten.Content += "0 min:sek";
            }
            else if (_amodel.IsDistanz)
            {
                aboveLabel_unten.Content = "0" + "m";
            }
            else
            {
                aboveLabel_unten.Content = "0 Runden";
            }



            aboveLabel_unten.Width = b.Width;
            aboveLabel_unten.Height = 24; // Setze eine feste Höhe für das Label
            aboveLabel_unten.HorizontalContentAlignment = HorizontalAlignment.Center;

            Canvas.SetLeft(aboveLabel_unten, 0);
            Canvas.SetBottom(aboveLabel_unten, b.Height); // Positioniere das Label über dem Balken
            canvascanvas.Children.Add(aboveLabel_unten);


            Label c = new Label();

            c.Height = 2;
            c.Width = canvascanvas.ActualWidth;
            c.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            Canvas.SetLeft(c, 0);
            Canvas.SetTop(c, 24);

            Label aboveLabel = new Label();


            if (_amodel.IsZeit)
            {
                double a = maxwert % 60;
                a = Math.Round(a, 0);
                double b2 = (maxwert - a) / 60;
                b2 = Math.Round(b2, 0);
                aboveLabel.Content += b2 + ":" + a;
            }
            else if (_amodel.IsDistanz)
            {
                aboveLabel.Content = maxwert + "m";
            }
            else
            {
                aboveLabel.Content = maxwert;
            }



            aboveLabel.Width = b.Width;
            aboveLabel.Height = 24; // Setze eine feste Höhe für das Label
            aboveLabel.HorizontalContentAlignment = HorizontalAlignment.Center;

            Canvas.SetLeft(aboveLabel, 0);
            Canvas.SetTop(aboveLabel, 4); // Positioniere das Label über dem Balken
            canvascanvas.Children.Add(aboveLabel);
            canvascanvas.Children.Add(c);
        }
    }
}
