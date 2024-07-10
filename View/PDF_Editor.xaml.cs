using System.Windows;

namespace Klimalauf
{
	/// <summary>
	/// Interaktionslogik für PDF_Editor.xaml
	/// </summary>
	public partial class PDF_Editor : Window
	{
		private PDFEditorModel? _pemodel;
		private MainViewModel? _mvmodel;
		private string? pfad;

		public PDF_Editor(Klasse klasse) : base()
		{
			InitializeComponent();
			_pemodel = FindResource("pemodel") as PDFEditorModel;
			_pemodel.Klasse = klasse;
			_pemodel.Format = new Format();

			ScannerName.Content =  $"{_mvmodel.Benutzer.Vorname}, {_mvmodel.Benutzer.Nachname}";

			using (var db = new LaufDBContext())
			{
				cbFormate.ItemsSource = db.Formate.ToList();
				cbBlattgroessee.ItemsSource = db.BlattGroessen.ToList();
				cbBlattgroessee.SelectedItem = _pemodel.Format.BlattGroesse ?? db.BlattGroessen.First(x=> x.Name == "A4");
				_pemodel.Format.BlattGroesse = cbBlattgroessee.SelectedItem as BlattGroesse;
				cbTyp.ItemsSource = Enum.GetValues(typeof(SchriftTyp));
				cbOrientierung.ItemsSource = Enum.GetValues(typeof(Orientierung));
			}
			cbTyp.SelectedIndex = 0;
			cbOrientierung.SelectedIndex = 0;


			btnSpeichern.Click += (s, e) =>
			{
				using (var db = new LaufDBContext())
				{
					if (db.Formate.Any(f => f.Name == _pemodel.Format.Name))
					{
						MessageBoxResult result = MessageBox.Show("Format überschreiben?", "Format existiert bereits", MessageBoxButton.YesNo);
						if (result == MessageBoxResult.No) return;
						db.Formate.Update(_pemodel.Format);
					}
					else
					{
						MessageBoxResult result = MessageBox.Show("Neues Format speichern?", "Format speichern", MessageBoxButton.YesNo);
						if (result == MessageBoxResult.No) return;
						_pemodel.Format.Id = 0;
						_pemodel.Format.BlattGroesse = db.BlattGroessen.First(x => x.Name == "A4");
						_pemodel.Format.BlattGroesseId = _pemodel.Format.BlattGroesse.Id;
						db.Formate.Add(_pemodel.Format);
					}
					db.SaveChanges();
					cbFormate.ItemsSource = db.Formate.ToList();
				}
			};

			btnNeuladen.Click += (s, e) =>
			{

				webView.Source = new Uri("about:blank");
				string pfad = PDFGenerator.BarcodesPDF(_pemodel.Klasse, _pemodel.Klasse.Schule.Name, _pemodel.Format);
				webView.Source = new Uri(pfad);
			};
			this.Loaded += (s, e) =>
			{
				webView.Source = new Uri("about:blank");
				string pfad = PDFGenerator.BarcodesPDF(_pemodel.Klasse, _pemodel.Klasse.Schule.Name, _pemodel.Format);
				webView.Source = new Uri(pfad);
			};


			cbFormate.SelectionChanged += (s, e) =>
			{
				_pemodel.Format = (Format)cbFormate.SelectedItem;
				cbTyp.SelectedIndex = _pemodel.Format.SchriftTyp switch
				{
					SchriftTyp.Normal => 0,
					SchriftTyp.Fett => 1,
					SchriftTyp.Kursiv => 2,
					SchriftTyp.FettKursiv => 3,
					_ => 0
				};
				cbBlattgroessee.SelectedIndex = _pemodel.Format.BlattGroesseId - 1;
				cbOrientierung.SelectedIndex = _pemodel.Format.Orientierung switch
				{
					Orientierung.Hochformat => 0,
					Orientierung.Querformat => 1,
					_ => 0
				};
			};

			cbBlattgroessee.SelectionChanged += (s, e) =>
						{
							_pemodel.Format.BlattGroesse = (BlattGroesse)cbBlattgroessee.SelectedItem;
						};

			cbTyp.SelectionChanged += (s, e) =>
			{
				_pemodel.Format.SchriftTyp = (SchriftTyp)cbTyp.SelectedIndex;
			};

			cbOrientierung.SelectionChanged += (s, e) =>
			{
				_pemodel.Format.Orientierung = (Orientierung)cbOrientierung.SelectedIndex;
			};
		}

	}
}
