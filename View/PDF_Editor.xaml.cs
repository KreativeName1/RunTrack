using System.Windows;

namespace Klimalauf
{
	/// <summary>
	/// Interaktionslogik für PDF_Editor.xaml
	/// </summary>
	public partial class PDF_Editor : Window
	{
		PDFEditorModel? m;
		string? pfad;

		public PDF_Editor(Klasse klasse, string vorname, string nachname) : base()
		{
			InitializeComponent();
			m = FindResource("pemodel") as PDFEditorModel;
			m.Klasse = klasse;
			m.Format = new Format();


			ScannerName.Content = vorname + " " + nachname;



			using (var db = new LaufDBContext())
			{
				cbFormate.ItemsSource = db.Formate.ToList();
				cbBlattgroessee.ItemsSource = db.BlattGroessen.ToList();
				cbBlattgroessee.SelectedItem = m.Format.BlattGroesse ?? db.BlattGroessen.First(x=> x.Name == "A4");
				m.Format.BlattGroesse = cbBlattgroessee.SelectedItem as BlattGroesse;
				cbTyp.ItemsSource = Enum.GetValues(typeof(SchriftTyp));
				cbOrientierung.ItemsSource = Enum.GetValues(typeof(Orientierung));
			}
			cbTyp.SelectedIndex = 0;
			cbOrientierung.SelectedIndex = 0;


			btnSpeichern.Click += (s, e) =>
			{
				using (var db = new LaufDBContext())
				{
					if (db.Formate.Any(f => f.Name == m.Format.Name))
					{
						MessageBoxResult result = MessageBox.Show("Format überschreiben?", "Format existiert bereits", MessageBoxButton.YesNo);
						if (result == MessageBoxResult.No) return;
						db.Formate.Update(m.Format);
					}
					else
					{
						MessageBoxResult result = MessageBox.Show("Neues Format speichern?", "Format speichern", MessageBoxButton.YesNo);
						if (result == MessageBoxResult.No) return;
						m.Format.Id = 0;
						m.Format.BlattGroesse = db.BlattGroessen.First(x => x.Name == "A4");
						m.Format.BlattGroesseId = m.Format.BlattGroesse.Id;
						db.Formate.Add(m.Format);
					}
					db.SaveChanges();
					cbFormate.ItemsSource = db.Formate.ToList();
				}
			};

			btnNeuladen.Click += (s, e) =>
			{

				webView.Source = new Uri("about:blank");
				string pfad = PDFGenerator.BarcodesPDF(m.Klasse, m.Klasse.Schule.Name, m.Format);
				webView.Source = new Uri(pfad);
			};
			this.Loaded += (s, e) =>
			{
				webView.Source = new Uri("about:blank");
				string pfad = PDFGenerator.BarcodesPDF(m.Klasse, m.Klasse.Schule.Name, m.Format);
				webView.Source = new Uri(pfad);
			};


			cbFormate.SelectionChanged += (s, e) =>
			{
				m.Format = (Format)cbFormate.SelectedItem;
				cbTyp.SelectedIndex = m.Format.SchriftTyp switch
				{
					SchriftTyp.Normal => 0,
					SchriftTyp.Fett => 1,
					SchriftTyp.Kursiv => 2,
					SchriftTyp.FettKursiv => 3,
					_ => 0
				};
				cbBlattgroessee.SelectedIndex = m.Format.BlattGroesseId - 1;
				cbOrientierung.SelectedIndex = m.Format.Orientierung switch
				{
					Orientierung.Hochformat => 0,
					Orientierung.Querformat => 1,
					_ => 0
				};
			};

			cbBlattgroessee.SelectionChanged += (s, e) =>
						{
							m.Format.BlattGroesse = (BlattGroesse)cbBlattgroessee.SelectedItem;
						};

			cbTyp.SelectionChanged += (s, e) =>
			{
				m.Format.SchriftTyp = (SchriftTyp)cbTyp.SelectedIndex;
			};

			cbOrientierung.SelectionChanged += (s, e) =>
			{
				m.Format.Orientierung = (Orientierung)cbOrientierung.SelectedIndex;
			};
		}

	}
}
