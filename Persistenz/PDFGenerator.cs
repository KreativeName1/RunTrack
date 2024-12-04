using iText.Barcodes;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using System.Reflection;
using System.Windows;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace RunTrack
{
    public class PDFGenerator
    {
        public static string Pfad = $"Temp/";
        public static PdfDocument? PDFDokument;
        public static Document? Dokument;
        public static string BarcodesPDF(List<Klasse> klassen, Format format)
        {
            string datei = DokumentErstellen(format);
            if (Dokument == null) return "";
            // Schule/Klasse Anzeigen
            foreach (Klasse klasse in klassen)
            {
                if (format.KopfAnzeigen)
                {
                    Dokument.Add(new Paragraph("Schule: " + klasse.Schule.Name).SetTextAlignment(TextAlignment.LEFT).SetBold().SetFontSize(14));
                    Dokument.Add(new Paragraph("Klasse: " + klasse.Name).SetTextAlignment(TextAlignment.LEFT).SetBold().SetFontSize(14));
                }

                // Tabelle erstellen
                int numColumns = format.SpaltenAnzahl;
                float columnWidth = format.ZellenBreite;
                Table table = new(UnitValue.CreatePointArray(Enumerable.Repeat(columnWidth, numColumns).ToArray()));

                // Tabelle Zentrieren oder nicht
                if (format.Zentriert) table.SetWidth(UnitValue.CreatePercentValue(100));
                else table.SetWidth(numColumns * columnWidth);

                table.SetHorizontalAlignment(HorizontalAlignment.LEFT);

                // Zellen erstellen
                for (int i = 0; i < klasse.Schueler.Count; i++)
                {

                    Cell cell = new();
                    cell.SetBorder(Border.NO_BORDER);
                    cell.SetTextAlignment(TextAlignment.CENTER);

                    // Barcode erstellen und in PDF einfügen
                    Barcode39 code39 = new(PDFDokument);
                    code39.SetCode(klasse.Schueler[i].Id.ToString().PadLeft(5, '0'));
                    code39.SetFont(null);
                    PdfFormXObject barcode = code39.CreateFormXObject(ColorConstants.BLACK, ColorConstants.BLACK, PDFDokument);
                    Image img = new(barcode);
                    img.SetHeight(format.ZellenHoehe - 10);
                    img.SetWidth(format.ZellenBreite - 10);
                    img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    cell.Add(img);

                    // Schülername und ID in PDF einfügen
                    Paragraph p = new(klasse.Schueler[i].Vorname + " " + klasse.Schueler[i].Nachname + " - " + klasse.Schueler[i].Id.ToString());
                    if (format.SchriftTyp == SchriftTyp.Fett) p.SetBold();
                    if (format.SchriftTyp == SchriftTyp.Kursiv) p.SetItalic();
                    if (format.SchriftTyp == SchriftTyp.FettKursiv) p.SetBold().SetItalic();
                    p.SetFontSize(format.SchriftGroesse);
                    p.SetTextAlignment(TextAlignment.CENTER);
                    cell.Add(p);

                    // Zellenabstand
                    cell.SetPaddingTop(format.ZellenAbstandHorizontal);
                    cell.SetPaddingRight(format.ZellenAbstandVertikal);

                    table.AddCell(cell);
                }

                Dokument.Add(table);

                if (klassen.IndexOf(klasse) < klassen.Count - 1)
                    Dokument.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }
            Dokument.Close();

            return datei;
        }

        public static string BarcodesPDFLaeufer(List<Laeufer> laeufer, Format format)
        {
            string datei = DokumentErstellen(format);
            if (Dokument == null) return "";

            // Tabelle erstellen
            int numColumns = format.SpaltenAnzahl;
            float columnWidth = format.ZellenBreite;
            Table table = new(UnitValue.CreatePointArray(Enumerable.Repeat(columnWidth, numColumns).ToArray()));

            // Tabelle Zentrieren oder nicht
            if (format.Zentriert) table.SetWidth(UnitValue.CreatePercentValue(100));
            else table.SetWidth(numColumns * columnWidth);

            table.SetHorizontalAlignment(HorizontalAlignment.LEFT);

            // Zellen erstellen
            foreach (Laeufer l in laeufer)
            {

                Cell cell = new();
                cell.SetBorder(Border.NO_BORDER);
                cell.SetTextAlignment(TextAlignment.CENTER);

                // Barcode erstellen und in PDF einfügen
                Barcode39 code39 = new(PDFDokument);
                code39.SetCode(l.Id.ToString().PadLeft(5, '0'));
                code39.SetFont(null);
                PdfFormXObject barcode = code39.CreateFormXObject(ColorConstants.BLACK, ColorConstants.BLACK, PDFDokument);
                Image img = new(barcode);
                img.SetHeight(format.ZellenHoehe - 10);
                img.SetWidth(format.ZellenBreite - 10);
                img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                cell.Add(img);

                // Läufername und ID in PDF einfügen
                Paragraph p = new(l.Vorname + " " + l.Nachname + " - " + l.Id.ToString());
                if (format.SchriftTyp == SchriftTyp.Fett) p.SetBold();
                if (format.SchriftTyp == SchriftTyp.Kursiv) p.SetItalic();
                if (format.SchriftTyp == SchriftTyp.FettKursiv) p.SetBold().SetItalic();
                p.SetFontSize(format.SchriftGroesse);
                p.SetTextAlignment(TextAlignment.CENTER);
                cell.Add(p);

                // Zellenabstand
                cell.SetPaddingTop(format.ZellenAbstandHorizontal);
                cell.SetPaddingRight(format.ZellenAbstandVertikal);

                table.AddCell(cell);
            }

            Dokument.Add(table);
            Dokument.Close();

            return datei;
        }

        public static string DokumentErstellen(Format format)
        {
            string name = DateTime.Now.ToString("dd.MM.yyyy") + "_" + new Random().Next(1000, 9999);
            string file = Pfad + $"{name}.pdf";
            Directory.CreateDirectory(Pfad);
            PDFDokument = new(new PdfWriter(file));
            if (format.BlattGroesse == null) format.BlattGroesse = new BlattGroesse(595f, 842f);
            // Blattgröße und Orientierung setzen
            PageSize pageSize = new(format.BlattGroesse.Breite, format.BlattGroesse.Hoehe);

            if (format.Orientierung == Orientierung.Hochformat) Dokument = new Document(PDFDokument, pageSize);
            else Dokument = new Document(PDFDokument, pageSize.Rotate());

            // Seitenrand
            Dokument.SetMargins(format.SeitenRandOben, format.SeitenRandRechts, format.SeitenRandUnten, format.SeitenRandLinks);
            return System.IO.Path.GetFullPath(file);
        }


        public static string SchuelerBewertungPDF(List<Schueler> schuelerListe, Format format, bool NeueSeiteProSchueler)
        {
            string datei = DokumentErstellen(format);
            if (Dokument == null) return string.Empty;
            if (format.SchriftTyp == SchriftTyp.Fett) Dokument.SetBold();
            if (format.SchriftTyp == SchriftTyp.Kursiv) Dokument.SetItalic();
            if (format.SchriftTyp == SchriftTyp.FettKursiv) Dokument.SetBold().SetItalic();
            Dokument.SetFontSize(format.SchriftGroesse);

            foreach (Schueler schueler in schuelerListe)
            {
                Dokument.Add(new Paragraph(DateTime.Now.ToString("dd.MM.yyyy")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse * 0.8f));
                Dokument.Add(new Paragraph(schueler.Klasse.Schule.Name + " " + schueler.Klasse.Name).SetTextAlignment(TextAlignment.CENTER).SetFontSize(format.SchriftGroesse * 1.2f));
                Dokument.Add(new Paragraph(schueler.Vorname + " " + schueler.Nachname).SetTextAlignment(TextAlignment.CENTER).SetBold().SetFontSize(format.SchriftGroesse * 1.5f));


                List<TimeSpan> rundenZeiten = new();

                for (int i = 1; i < schueler.Runden.Count; i++)
                {
                    TimeSpan rundenzeit = schueler.Runden[i].Zeitstempel - schueler.Runden[i - 1].Zeitstempel;
                    rundenZeiten.Add(rundenzeit);
                }

                if (rundenZeiten.Count > 0)
                {
                    TimeSpan schnellsteRunde = rundenZeiten[0];
                    int indexSchnellsteRunde = 0;

                    for (int i = 1; i < rundenZeiten.Count; i++)
                    {

                        if (rundenZeiten[i] < schnellsteRunde)
                        {
                            schnellsteRunde = rundenZeiten[i];
                            indexSchnellsteRunde = i;
                        }
                    }

                    Dokument.Add(new Paragraph("Schnellste Runde: Runde " + (indexSchnellsteRunde + 1) + " mit " + schnellsteRunde.ToString(@"mm\:ss")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse));
                    Dokument.Add(new Paragraph("Gelaufene Meter: " + ((rundenZeiten.Count - 1) * schueler.Klasse.RundenArt.LaengeInMeter).ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse));
                    Dokument.Add(new Paragraph("Anzahl Runden: " + (rundenZeiten.Count - 1).ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse));
                }
                else
                {
                    Dokument.Add(new Paragraph("Keine Rundenzeiten vorhanden").SetTextAlignment(TextAlignment.LEFT).SetFontSize(12));
                }

                Table table = new(2);

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

                Dokument.Add(table);

                if (schuelerListe.IndexOf(schueler) < schuelerListe.Count - 1 && NeueSeiteProSchueler)
                    Dokument.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }
            Dokument.Close();

            return datei;
        }



        public static string AuswertungListe(List<object> liste, Format format, string auswertungArt)
        {
            string datei = DokumentErstellen(format);
            if (Dokument == null) return string.Empty;

            PropertyInfo[] propertyInfos = liste[0].GetType().GetProperties();
            Table table = new(100);

            if (format.Zentriert) table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            else table.SetHorizontalAlignment(HorizontalAlignment.LEFT);


            if (format.SchriftTyp == SchriftTyp.Fett) Dokument.SetBold();
            if (format.SchriftTyp == SchriftTyp.Kursiv) Dokument.SetItalic();
            if (format.SchriftTyp == SchriftTyp.FettKursiv) Dokument.SetBold().SetItalic();
            Dokument.SetFontSize(format.SchriftGroesse);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Cell cell = new();
                if (propertyInfo.Name == "Bewertung") cell.Add(new Paragraph(auswertungArt));
                else cell.Add(new Paragraph(propertyInfo.Name));
                cell.SetPaddings(0, 5, 0, 5);
                table.AddHeaderCell(cell);
            }

            foreach (object obj in liste)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    Cell cell = new();
                    cell.Add(new Paragraph(propertyInfo?.GetValue(obj)?.ToString()).SetFontSize(format.SchriftGroesse));
                    cell.SetPaddings(0, 5, 0, 5);
                    table.AddCell(cell);
                }
                table.StartNewRow();
            }

            if (format.KopfAnzeigen) Dokument.Add(new Paragraph(DateTime.Now.ToString("dd.MM.yyy HH:mm:ss")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse * 0.8f));
            Dokument.Add(table);

            Dokument.Close();
            return datei;
        }


        public static string Urkunde(List<Urkunde> liste, Format format)
        {
            string datei = DokumentErstellen(format);
            if (Dokument == null) return string.Empty;

            int lineWidth = 200;
            Color primaryColor = new DeviceRgb(0, 102, 204); // Ein eleganter Blauton
            Color defaultColor = new DeviceRgb(0, 87, 70); // Standardfarbe für Platzierungen ab dem 3. Platz
            Color fontColor = new DeviceRgb(42, 53, 106); // Standardfarbe für Platzierungen ab dem 3. Platz

            foreach (Urkunde obj in liste)
            {
                // Hintergrund mit dezentem Wasserzeichen
                var uri = new Uri("pack://application:,,,/Images/watermark.png");
                var resourceStream = Application.GetResourceStream(uri).Stream;

                using (var memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);
                    var imageData = ImageDataFactory.Create(memoryStream.ToArray());
                    var image = new Image(imageData)
                        .SetFixedPosition(0, 0)
                        .ScaleToFit(format.BlattGroesse.Breite, format.BlattGroesse.Hoehe)
                        .SetOpacity(0.1f);

                    Dokument.Add(image);
                }

                // Haupttitel
                Dokument.Add(new Paragraph("Urkunde")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetFontSize(format.SchriftGroesse * 3)
                    .SetFontColor(primaryColor)
                    .SetMarginTop(20));

                // Veranstaltung
                Dokument.Add(new Paragraph("Verliehen bei dem")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(format.SchriftGroesse * 1.1f));
                Dokument.Add(new Paragraph(obj.LaufName)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetFontSize(format.SchriftGroesse * 1.25f)
                    .SetFontColor(fontColor));

                // Trennlinie
                Dokument.Add(new LineSeparator(new SolidLine())
                    .SetMarginLeft(lineWidth)
                    .SetMarginRight(lineWidth)
                    .SetMarginTop((float)format.ZeilenAbstand)
                    .SetMarginBottom((float)format.ZeilenAbstand)
                    .SetStrokeColor(primaryColor));

                // Teilnehmername
                Dokument.Add(new Paragraph("an")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(format.SchriftGroesse * 1.1f));
                Dokument.Add(new Paragraph(obj.Name)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetFontSize(format.SchriftGroesse * 1.25f)
                    .SetFontColor(fontColor));

                Dokument.Add(new Paragraph("mit dem")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(format.SchriftGroesse * 1.1f));

                // Platzierung
                int platzierung;
                if (int.TryParse(obj.Platzierung, out platzierung) && platzierung >= 4)
                {
                    Dokument.Add(new Paragraph($"{obj.Platzierung}. Platz")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBold()
                        .SetFontSize(format.SchriftGroesse * 1.5f)
                        .SetFontColor(defaultColor));
                }
                else
                {
                    Dokument.Add(new Paragraph($"{obj.Platzierung}. Platz")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetBold()
                        .SetFontSize(format.SchriftGroesse * 1.5f)
                        .SetFontColor(ColorConstants.ORANGE));
                }

                // Tabelle für zusätzliche Informationen
                Table infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3 })).UseAllAvailableWidth();
                infoTable.SetMarginTop(140);

                infoTable.AddCell(new Cell().Add(new Paragraph("Rundenart:").SetBold()).SetBorder(Border.NO_BORDER));
                infoTable.AddCell(new Cell().Add(new Paragraph(obj.Rundenart)).SetBorder(Border.NO_BORDER));

                infoTable.AddCell(new Cell().Add(new Paragraph("Schnellste Runde:").SetBold()).SetBorder(Border.NO_BORDER));
                infoTable.AddCell(new Cell().Add(new Paragraph(obj.SchnellsteRunde.ToString(@"mm\:ss") + " min")).SetBorder(Border.NO_BORDER));

                infoTable.AddCell(new Cell().Add(new Paragraph("Gelaufene Meter:").SetBold()).SetBorder(Border.NO_BORDER));
                infoTable.AddCell(new Cell().Add(new Paragraph(obj.GelaufeneMeter.ToString() + " m")).SetBorder(Border.NO_BORDER));

                infoTable.AddCell(new Cell().Add(new Paragraph("Anzahl Runden:").SetBold()).SetBorder(Border.NO_BORDER));
                infoTable.AddCell(new Cell().Add(new Paragraph(obj.AnzahlRunden.ToString() + " Runde/n")).SetBorder(Border.NO_BORDER));

                Dokument.Add(infoTable);

                // Signaturbereich ans untere Ende des Dokuments verschieben
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1.25f, 2 })).UseAllAvailableWidth();

                Cell leftCell = new Cell().Add(new Paragraph("Datum und Ort").SetFontSize(format.SchriftGroesse));
                leftCell.SetTextAlignment(TextAlignment.CENTER)
                        .SetBorder(Border.NO_BORDER)
                        .SetBorderTop(new SolidBorder(1));

                Cell spacerCell = new Cell();
                spacerCell.SetBorder(Border.NO_BORDER);

                Cell rightCell = new Cell().Add(new Paragraph("Unterschrift").SetFontSize(format.SchriftGroesse));
                rightCell.SetTextAlignment(TextAlignment.CENTER)
                         .SetBorder(Border.NO_BORDER)
                         .SetBorderTop(new SolidBorder(1));

                table.AddCell(leftCell);
                table.AddCell(spacerCell);
                table.AddCell(rightCell);

                table.SetMarginTop(50);

                // Den Signaturbereich ans Ende verschieben
                Dokument.Add(new Paragraph().SetHeight(220)); // Platzhalter, um Abstand zu schaffen
                Dokument.Add(table);

                // Seitenumbruch für nächste Urkunde
                if (liste.IndexOf(obj) < liste.Count - 1) Dokument.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            }

            Dokument.Close();
            return datei;
        }





    }
}
