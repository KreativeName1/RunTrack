using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
using System.Windows.Shapes;
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
            foreach (object item in _amodel.Liste)
            {
                string name, bewertung, bewertung2 = "";
                int bewertungzr = 0;
                name = (string) item.GetType().GetProperty("Name").GetValue(item, null);
                bewertung = (string) item.GetType().GetProperty("Bewertung").GetValue(item, null);

                if (_amodel.IsDistanz)
                {
                    Diagrammtitel = "Distanz";
                        foreach(char a in bewertung)
                        {
                            if(a != '.' &&  a != ' ' && a != 'm')
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
                      

                }else if(_amodel.IsAnzahl)
                {
                    Diagrammtitel = "RundenAnzahl";
                    bewertungzr = Convert.ToInt32(bewertung);
                }
                else if(_amodel.IsZeit)
                {
                    Diagrammtitel = "Schnellste Runde";
                    string[] split = new string[2];
                    split = bewertung.Split(':');
                    split[0] = split[0].Trim();
                    split[1] = split[1].Trim();
                    bewertungzr += (Convert.ToInt32(split[0]))*60;
                    bewertungzr += Convert.ToInt32(split[1]);
                }

                DiagrammWert d = new DiagrammWert();
                d.name = name;
                d.wert = bewertungzr;
                Random r = new Random();
                d.Farbe = new SolidColorBrush(Color.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255)));
                diagrammliste.Add(d);
     

            }
            label.Content = Diagrammtitel;

            drawCanvas();
            
          
        }

        private void drawCanvas()
        {
            Diagrammcanvas.Children.Clear();

            double maxWert = 0;

            foreach (DiagrammWert dgwert in diagrammliste)
            {
                if (dgwert.wert > maxWert)
                {
                    maxWert = dgwert.wert;
                }
              

                // Oder Alternative Lösung
                // maxWert = Math.Max(maxWert, data.Wert);
            }

            double canvasHeight = Diagrammcanvas.ActualHeight;
            
            double canvasWidth = Diagrammcanvas.ActualWidth - 20;
            if(canvasHeight > 10 && canvasWidth > 10) { 
            double balkenAbstand = 10;
            double balkenBreite = canvasWidth / diagrammliste.Count;

            if(canvasWidth > 0)
            {

           
            for (int i = 0; i < diagrammliste.Count; i++)
            {
                    DiagrammWert d = diagrammliste[i];
                double balkenHoehe = (d.wert / maxWert) * (canvasHeight - 10);

                // Überprüfung, ob Balkenhöhe kleiner als 20 ist
                if (balkenHoehe < 22)
                {
                    Label aboveLabel = new Label();
                    aboveLabel.Content = d.name;
                    aboveLabel.Width = balkenBreite;
                    aboveLabel.Height = 24; // Setze eine feste Höhe für das Label
                    aboveLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    aboveLabel.Tag = d; // Setze den Datenpunkt als Tag
                   


                    Canvas.SetLeft(aboveLabel, i * (balkenBreite + balkenAbstand) + 10);
                    Canvas.SetBottom(aboveLabel, balkenHoehe + 5); // Positioniere das Label über dem Balken
                    Diagrammcanvas.Children.Add(aboveLabel);
                }

                    Label label = new Label()
                    {
                        Background = d.Farbe,
                        Content = d.name,
                    Width = balkenBreite,
                    Height = balkenHoehe,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = d, // Setze den Datenpunkt als Tag
                    Cursor = Cursors.Hand
                };

                

                Canvas.SetLeft(label, i * (balkenBreite + balkenAbstand) + 10);
                Canvas.SetBottom(label, 0);
                    
                    
                    
               
                    Diagrammcanvas.Children.Add(label);
                }
                }
            }
        }
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            drawCanvas();
        }

        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            drawCanvas();
        }
    }
}
