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
using Image = iText.Layout.Element.Image;

namespace Klimalauf
{
	public class PDFGenerator
	{
		public static string BarcodesPDF(Klasse klasse, string schulename, Format f)
		{
			// PDF Dokument erstellen
			string path = $"Temp/";
			Directory.CreateDirectory(path);
			PdfDocument pdf = new(new PdfWriter(path + $"/{klasse.Name}.pdf"));

			// Blattgröße und Orientierung setzen
			PageSize pageSize = new(f.BlattGroesse.Breite, f.BlattGroesse.Hoehe);
			Document document;
			if (f.Orientierung == Orientierung.Hochformat) document = new Document(pdf, pageSize);
			else document = new Document(pdf, pageSize.Rotate());

			// Seitenrand
			document.SetMargins(f.SeitenRandOben, f.SeitenRandRechts, f.SeitenRandUnten, f.SeitenRandLinks);

			// Schule/Klasse Anzeigen
			if (f.KopfAnzeigen)
			{
				document.Add(new Paragraph("Schule: " + schulename).SetTextAlignment(TextAlignment.LEFT).SetBold().SetFontSize(14));
				document.Add(new Paragraph("Klasse: " + klasse.Name).SetTextAlignment(TextAlignment.LEFT).SetBold().SetFontSize(14));
			}

			// Tabelle erstellen
			int numColumns = f.SpaltenAnzahl;
			float columnWidth = f.ZellenBreite;
			Table table = new Table(UnitValue.CreatePointArray(Enumerable.Repeat(columnWidth, numColumns).ToArray()));
			
			// Tabelle Zentrieren oder nicht
			if (f.Zentriert) table.SetWidth(UnitValue.CreatePercentValue(100));
			else table.SetWidth(numColumns * columnWidth);
			
			table.SetHorizontalAlignment(HorizontalAlignment.LEFT);

			// Zellen erstellen
			for (int i = 0; i < klasse.Schueler.Count; i++)
			{

				Cell cell = new();
				cell.SetBorder(Border.NO_BORDER);
				cell.SetTextAlignment(TextAlignment.CENTER);

				// Barcode erstellen und in PDF einfügen
				Barcode39 code39 = new Barcode39(pdf);
				code39.SetCode(klasse.Schueler[i].Id.ToString().PadLeft(5, '0'));
				code39.SetFont(null);
				PdfFormXObject barcode = code39.CreateFormXObject(ColorConstants.BLACK, ColorConstants.BLACK, pdf);
				Image img = new Image(barcode);
				img.SetHeight(f.ZellenHoehe - 10);
				img.SetWidth(f.ZellenBreite - 10);
				img.SetHorizontalAlignment(HorizontalAlignment.CENTER);
				cell.Add(img);

				// Schülername und ID in PDF einfügen
				Paragraph p = new Paragraph(klasse.Schueler[i].Vorname + " " + klasse.Schueler[i].Nachname + " - " + klasse.Schueler[i].Id.ToString());
				if (f.SchriftTyp == SchriftTyp.Fett) p.SetBold();
				if (f.SchriftTyp == SchriftTyp.Kursiv) p.SetItalic();
				if (f.SchriftTyp == SchriftTyp.FettKursiv) p.SetBold().SetItalic();
				p.SetFontSize(f.SchriftGroesse);
				p.SetTextAlignment(TextAlignment.CENTER);
				cell.Add(p);

				// Zellenabstand
				cell.SetPaddingTop(f.ZellenAbstandHorizontal);
				cell.SetPaddingRight(f.ZellenAbstandVertikal);

				table.AddCell(cell);
			}

			document.Add(table);
			document.Close();

			// Pfad der Datei
			return System.IO.Path.GetFullPath(path + $"/{klasse.Name}.pdf");
		}


		public static string SchuelerBewertungPDF(Schueler schueler, Format format)
		{
			string path = $"Temp";
			Directory.CreateDirectory(path);

			PdfDocument pdf = new PdfDocument(new PdfWriter(path + $"/{schueler.Vorname} {schueler.Nachname}.pdf"));
            // Blattgröße und Orientierung setzen
            PageSize pageSize = new(format.BlattGroesse.Breite, format.BlattGroesse.Hoehe);
            Document document;
            if (format.Orientierung == Orientierung.Hochformat) document = new Document(pdf, pageSize);
            else document = new Document(pdf, pageSize.Rotate());

            // Seitenrand
            document.SetMargins(format.SeitenRandOben, format.SeitenRandRechts, format.SeitenRandUnten, format.SeitenRandLinks);
            if (format.SchriftTyp == SchriftTyp.Fett) document.SetBold();
            if (format.SchriftTyp == SchriftTyp.Kursiv) document.SetItalic();
            if (format.SchriftTyp == SchriftTyp.FettKursiv) document.SetBold().SetItalic();
            document.SetFontSize(format.SchriftGroesse);

            document.Add(new Paragraph(DateTime.Now.ToString("dd.MM.yyyy")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse * 0.8f));
            document.Add(new Paragraph(schueler.Klasse.Schule.Name + " " + schueler.Klasse.Name).SetTextAlignment(TextAlignment.CENTER).SetFontSize(format.SchriftGroesse * 1.2f));
            document.Add(new Paragraph(schueler.Vorname + " " + schueler.Nachname).SetTextAlignment(TextAlignment.CENTER).SetBold().SetFontSize(format.SchriftGroesse * 1.5f));


            List<TimeSpan> rundenZeiten = new List<TimeSpan>();

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

				document.Add(new Paragraph("Schnellste Runde: Runde " + (indexSchnellsteRunde + 1) + " mit " + schnellsteRunde.ToString(@"mm\:ss")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse));
				document.Add(new Paragraph("Gelaufene Meter: " + ((rundenZeiten.Count-1) * schueler.Klasse.RundenArt.LaengeInMeter).ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse));
				document.Add(new Paragraph("Anzahl Runden: " + (rundenZeiten.Count-1).ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(format.SchriftGroesse));

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

            return System.IO.Path.GetFullPath(path + $"/{schueler.Vorname} {schueler.Nachname}.pdf");


        }
	}
}
