using iText.Barcodes;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;

namespace Klimalauf
{
    public class PDFGenerator
    {
        public static void BarcodesPDF(Klasse klasse, string schulename)
        {
            string path = $"Dokumente/Barcodes/{schulename}";
            Directory.CreateDirectory(path);

            PdfDocument pdf = new PdfDocument(new PdfWriter(path + $"/{klasse.Name}.pdf"));
            Document document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            document.Add(new Paragraph("Schule: " + schulename).SetTextAlignment(TextAlignment.LEFT).SetBold().SetFontSize(14));
            document.Add(new Paragraph("Klasse: " + klasse.Name).SetTextAlignment(TextAlignment.LEFT).SetBold().SetFontSize(14));
            int numColumns = 2;
            float columnWidth = (PageSize.A4.GetWidth() - document.GetLeftMargin() - document.GetRightMargin()) / numColumns;
            Table table = new Table(UnitValue.CreatePointArray(Enumerable.Repeat(columnWidth, numColumns).ToArray()));
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.SetMarginTop(10);
            table.SetMarginBottom(10);

            for (int i = 0; i < klasse.Schueler.Count; i++)
            {
                Cell cell = new();
                cell.SetBorder(Border.NO_BORDER);
                cell.SetTextAlignment(TextAlignment.CENTER);
                cell.SetPaddingTop(10);
                Barcode39 code39 = new Barcode39(pdf);
                code39.SetCode(klasse.Schueler[i].Id.ToString().PadLeft(5, '0'));
                code39.SetFont(null);
                PdfFormXObject barcode = code39.CreateFormXObject(ColorConstants.BLACK, ColorConstants.BLACK, pdf);
                Image img = new Image(barcode);
                img.SetHeight(50);
                img.SetWidth(200);
                img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell.Add(img);
                Paragraph p = new Paragraph(klasse.Schueler[i].Name + " - " + klasse.Schueler[i].Id.ToString());
                p.SetBold();
                p.SetTextAlignment(TextAlignment.CENTER);
                cell.Add(p);
                table.AddCell(cell);

            }
            document.Add(table);

            document.Close();
        }


        public static void SchuelerBewertungPDF(Schueler schueler, string schulename)
        {
            string path = $"Dokumente/Bewertungen/{schulename}/{schueler.Klasse.Name}";
            Directory.CreateDirectory(path);

            PdfDocument pdf = new PdfDocument(new PdfWriter(path + $"/{schueler.Name}.pdf"));
            Document document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            document.Add(new Paragraph(schulename + " " + schueler.Klasse.Name).SetTextAlignment(TextAlignment.CENTER).SetFontSize(12));
            document.Add(new Paragraph(schueler.Name).SetTextAlignment(TextAlignment.CENTER).SetBold().SetFontSize(14));
            document.Add(new Paragraph("Datum: " + DateTime.Now.ToString("dd.MM.yyyy")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));

            List<TimeSpan> rundenZeiten = new List<TimeSpan>();

            for (int i = 1; i < schueler.Runden.Count; i++)
            {
                TimeSpan rundenzeit = schueler.Runden[i].Zeitstempel - schueler.Runden[i - 1].Zeitstempel;
                rundenZeiten.Add(rundenzeit);
            }

            if (rundenZeiten.Count > 0)
            {
                TimeSpan schnellsteRunde = rundenZeiten[0];
                TimeSpan langsamsteRunde = rundenZeiten[0];
                int indexSchnellsteRunde = 0;
                int indexLangsamsteRunde = 0;
                TimeSpan gesamtZeit = new TimeSpan(0, 0, 0);

                for (int i = 1; i < rundenZeiten.Count; i++)
                {
                    gesamtZeit += rundenZeiten[i];

                    if (rundenZeiten[i] < schnellsteRunde)
                    {
                        schnellsteRunde = rundenZeiten[i];
                        indexSchnellsteRunde = i;
                    }

                    if (rundenZeiten[i] > langsamsteRunde)
                    {
                        langsamsteRunde = rundenZeiten[i];
                        indexLangsamsteRunde = i;
                    }
                }

                TimeSpan durchschnittsZeit = new TimeSpan(gesamtZeit.Ticks / rundenZeiten.Count);

                document.Add(new Paragraph("Schnellste Runde: Runde " + (indexSchnellsteRunde + 1) + " mit " + schnellsteRunde).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));
                document.Add(new Paragraph("Langsamste Runde: Runde " + (indexLangsamsteRunde + 1) + " mit " + langsamsteRunde).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));
                document.Add(new Paragraph("Durchschnittszeit: " + durchschnittsZeit.ToString(@"hh\:mm\:ss")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));
                document.Add(new Paragraph("Gelaufene Meter: " + (rundenZeiten.Count * schueler.Klasse.RundenArt.LaengeInMeter).ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));

            }
            else
            {
                document.Add(new Paragraph("Keine Rundenzeiten vorhanden").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));
            }



            Table table = new Table(2);

            Cell cell = new();
            cell.Add(new Paragraph("Runde"));
            table.AddHeaderCell(cell);
            Cell cell2 = new();
            cell2.Add(new Paragraph("Zeit"));
            table.AddHeaderCell(cell2);


            for (int i = 0; i < schueler.Runden.Count; i++)
            {
                cell = new();
                cell.Add(new Paragraph($"{i + 1}"));
                if (i < schueler.Runden.Count - 1) table.AddCell(cell);
                cell = new();
                if (i < schueler.Runden.Count - 1)
                {
                    cell.Add(new Paragraph(rundenZeiten[i].ToString(@"hh\:mm\:ss")));
                }
                else
                {
                    cell.Add(new Paragraph(""));
                }

                if (i < schueler.Runden.Count - 1) table.AddCell(cell);
            }

            document.Add(table);

            document.Close();


        }
    }
}
