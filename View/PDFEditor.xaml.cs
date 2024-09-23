using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
	public partial class PDFEditor : Page
	{
		private PDFEditorModel? _pemodel;
		private MainModel? _pmodel;
		private string? _wertungArt;

		public PDFEditor(Klasse klasse) : base()
		{
			InitializeComponent();
			_pemodel = FindResource("pemodel") as PDFEditorModel ?? new PDFEditorModel();
			_pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
			Reset();
			_pemodel.Klasse = klasse;
			init();
		}

		public PDFEditor(List<Urkunde> urkunden) : base()
		{
			InitializeComponent();
			_pemodel = FindResource("pemodel") as PDFEditorModel ?? new PDFEditorModel();
			_pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
			Reset();
			_pemodel.Urkunden = new ObservableCollection<Urkunde>(urkunden);
			init();

			lblSpalten.Visibility = Visibility.Collapsed;
			txtSpalten.Visibility = Visibility.Collapsed;
			lblBarcodeGroesse.Visibility = Visibility.Collapsed;
			borBarcodeGroesse.Visibility = Visibility.Collapsed;
			lblBarcodeAbstand.Visibility = Visibility.Collapsed;
			borBarcodeAbstand.Visibility = Visibility.Collapsed;
			lblZeilenAbstand.Visibility = Visibility.Visible;
			txtZeilenAbstand.Visibility = Visibility.Visible;
			spKopf.Visibility = Visibility.Collapsed;
			spZentriert.Visibility = Visibility.Collapsed;
		}

		public PDFEditor(List<Schueler> schueler) : base()
		{
			InitializeComponent();
			_pemodel = FindResource("pemodel") as PDFEditorModel ?? new PDFEditorModel();
			_pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
			Reset();
			_pemodel.Schueler = new ObservableCollection<Schueler>(schueler);

			for (int i = PanelRight.Children.Count - 1; i > 0; i--)
			{
				PanelRight.Children.RemoveAt(i);
			}
			SchuelerBewertungPanel.Visibility = Visibility.Visible;
			init();
		}

		public PDFEditor(List<object> liste, string wertungArt) : base()
		{
			InitializeComponent();
			_pemodel = FindResource("pemodel") as PDFEditorModel ?? new PDFEditorModel();
			_pmodel = FindResource("pmodel") as MainModel ?? new MainModel();
			Reset();
			_wertungArt = wertungArt;

			_pemodel.Liste = new ObservableCollection<object>(liste);

			lblSpalten.Visibility = Visibility.Collapsed;
			txtSpalten.Visibility = Visibility.Collapsed;
			lblBarcodeGroesse.Visibility = Visibility.Collapsed;
			borBarcodeGroesse.Visibility = Visibility.Collapsed;
			lblBarcodeAbstand.Visibility = Visibility.Collapsed;
			borBarcodeAbstand.Visibility = Visibility.Collapsed;
			init();
		}

		public void Reset()
		{
			if (_pemodel == null) return;
			_pemodel.Liste = null;
			_pemodel.Klasse = null;
			_pemodel.Schueler = null;
			_pemodel.Urkunden = null;
		}

		public void init()
		{
			if (_pemodel == null) return;

			_pemodel.Format = new Format();

			using (var db = new LaufDBContext())
			{
				cbFormate.ItemsSource = db.Formate.ToList();
				cbBlattgroessee.ItemsSource = db.BlattGroessen.ToList();
				cbBlattgroessee.SelectedItem = _pemodel.Format.BlattGroesse ?? db.BlattGroessen.First(x => x.Name == "A4");
				_pemodel.Format.BlattGroesse = cbBlattgroessee.SelectedItem as BlattGroesse ?? db.BlattGroessen.First(x => x.Name == "A4");
				cbTyp.ItemsSource = Enum.GetValues(typeof(SchriftTyp));
				cbOrientierung.ItemsSource = Enum.GetValues(typeof(Orientierung));
			}

			cbTyp.SelectedIndex = 0;
			cbOrientierung.SelectedIndex = 0;

			this.Loaded += (s, e) =>
			{
				if (_pemodel == null) return;
				// Webview mit PDF füllen
				webView.Source = new Uri("about:blank");
				string pfad;
				if (_pemodel.Klasse != null) pfad = PDFGenerator.BarcodesPDF(_pemodel.Klasse, _pemodel.Klasse.Schule.Name, _pemodel.Format);
				else if (_pemodel.Liste != null) pfad = PDFGenerator.AuswertungListe(_pemodel.Liste.ToList(), _pemodel.Format, _wertungArt ?? string.Empty);
				else if (_pemodel.Urkunden != null) pfad = PDFGenerator.Urkunde(_pemodel.Urkunden.ToList(), _pemodel.Format);
				else pfad = PDFGenerator.SchuelerBewertungPDF(new List<Schueler>(_pemodel.Schueler ?? new()), _pemodel.Format, _pemodel.NeueSeiteProSchueler);
				webView.Source = new Uri(pfad);

				webView.ZoomFactor = 0.62;
			};

			btnSpeichern.Click += (s, e) =>
			{
				using (var db = new LaufDBContext())
				{
					if (db.Formate.Any(f => f.Name == _pemodel.Format.Name))
					{
						PopupResult result = new Popup().Display("Format existiert bereits", "Möchten Sie das Format überschreiben?", PopupType.Warning, PopupButtons.YesNo);
						if (result.Result == false) return;
						db.Formate.Update(_pemodel.Format);
					}
					else
					{
						PopupResult result = new Popup().Display("Neues Format speichern?", "Möchten Sie das Format speichern?", PopupType.Question, PopupButtons.YesNo);
						if (result.Result == false) return;
						_pemodel.Format.Id = 0;
						_pemodel.Format.BlattGroesse = db.BlattGroessen.First(x => x.Name == "A4");
						_pemodel.Format.BlattGroesseId = _pemodel.Format.BlattGroesse.Id;
						db.Formate.Add(_pemodel.Format);
					}
					db.SaveChanges();
					cbFormate.ItemsSource = db.Formate.ToList();
				}
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
				AktualisierePDF();
			};

			cbBlattgroessee.SelectionChanged += (s, e) =>
			{
				_pemodel.Format.BlattGroesse = (BlattGroesse)cbBlattgroessee.SelectedItem;
				AktualisierePDF();
			};

			cbTyp.SelectionChanged += (s, e) =>
			{
				_pemodel.Format.SchriftTyp = (SchriftTyp)cbTyp.SelectedIndex;
				AktualisierePDF();
			};

			cbOrientierung.SelectionChanged += (s, e) =>
			{
				_pemodel.Format.Orientierung = (Orientierung)cbOrientierung.SelectedIndex;
				AktualisierePDF();
			};

			cbNeueSeite.Unchecked += (s, e) =>
			{
				AktualisierePDF();
			};
			cbNeueSeite.Checked += (s, e) =>
			{
				AktualisierePDF();
			};

			// Speichere die aktuellen Werte
			string currentOben = txtOben.Text;
			string currentUnten = txtUnten.Text;
			string currentLinks = txtLinks.Text;
			string currentRechts = txtRechts.Text;
			string currentAbstandHorizontal = txtAbstandHorizontal.Text;
			string currentAbstandVertikal = txtAbstandVertikal.Text;
			string currentBreite = txtBreite.Text;
			string currentHöhe = txtHöhe.Text;
			string currentGroesse = txtGroesse.Text;
			string currentSpalten = txtSpalten.Text;
			bool currentKopf = chkKopf.IsChecked ?? false;
			bool currentZentriert = chkZentriert.IsChecked ?? false;

			// Überprüfe auf Änderungen und aktualisiere bei Bedarf
			txtOben.LostFocus += (s, e) =>
			{
				if (txtOben.Text != currentOben)
				{
					currentOben = txtOben.Text;
					AktualisierePDF();
				}
			};
			txtUnten.LostFocus += (s, e) =>
			{
				if (txtUnten.Text != currentUnten)
				{
					currentUnten = txtUnten.Text;
					AktualisierePDF();
				}
			};
			txtLinks.LostFocus += (s, e) =>
			{
				if (txtLinks.Text != currentLinks)
				{
					currentLinks = txtLinks.Text;
					AktualisierePDF();
				}
			};
			txtRechts.LostFocus += (s, e) =>
			{
				if (txtRechts.Text != currentRechts)
				{
					currentRechts = txtRechts.Text;
					AktualisierePDF();
				}
			};
			txtAbstandHorizontal.LostFocus += (s, e) =>
			{
				if (txtAbstandHorizontal.Text != currentAbstandHorizontal)
				{
					currentAbstandHorizontal = txtAbstandHorizontal.Text;
					AktualisierePDF();
				}
			};
			txtAbstandVertikal.LostFocus += (s, e) =>
			{
				if (txtAbstandVertikal.Text != currentAbstandVertikal)
				{
					currentAbstandVertikal = txtAbstandVertikal.Text;
					AktualisierePDF();
				}
			};
			txtBreite.LostFocus += (s, e) =>
			{
				if (txtBreite.Text != currentBreite)
				{
					currentBreite = txtBreite.Text;
					AktualisierePDF();
				}
			};
			txtHöhe.LostFocus += (s, e) =>
			{
				if (txtHöhe.Text != currentHöhe)
				{
					currentHöhe = txtHöhe.Text;
					AktualisierePDF();
				}
			};
			txtGroesse.LostFocus += (s, e) =>
			{
				if (txtGroesse.Text != currentGroesse)
				{
					currentGroesse = txtGroesse.Text;
					AktualisierePDF();
				}
			};
			txtSpalten.LostFocus += (s, e) =>
			{
				if (txtSpalten.Text != currentSpalten)
				{
					currentSpalten = txtSpalten.Text;
					AktualisierePDF();
				}
			};

			chkKopf.Checked += (s, e) =>
			{
				if (chkKopf.IsChecked != currentKopf)
				{
					currentKopf = chkKopf.IsChecked ?? false;
					AktualisierePDF();
				}
			};
			chkKopf.Unchecked += (s, e) =>
			{
				if (chkKopf.IsChecked != currentKopf)
				{
					currentKopf = chkKopf.IsChecked ?? false;
					AktualisierePDF();
				}
			};
			chkZentriert.Checked += (s, e) =>
			{
				if (chkZentriert.IsChecked != currentZentriert)
				{
					currentZentriert = chkZentriert.IsChecked ?? false;
					AktualisierePDF();
				}
			};
			chkZentriert.Unchecked += (s, e) =>
			{
				if (chkZentriert.IsChecked != currentZentriert)
				{
					currentZentriert = chkZentriert.IsChecked ?? false;
					AktualisierePDF();
				}
			};

			btnNeuladen.Click += (s, e) =>
			{
				AktualisierePDF();
			};
		}

		private void AktualisierePDF()
		{
			if (_pemodel == null) return;
			webView.Source = new Uri("about:blank");
			string pfad;
			if (_pemodel.Klasse != null) pfad = PDFGenerator.BarcodesPDF(_pemodel.Klasse, _pemodel.Klasse.Schule.Name, _pemodel.Format ?? new());
			else if (_pemodel.Liste != null) pfad = PDFGenerator.AuswertungListe(_pemodel.Liste.ToList(), _pemodel.Format ?? new(), _wertungArt ?? string.Empty);
			else if (_pemodel.Urkunden != null) pfad = PDFGenerator.Urkunde(_pemodel.Urkunden.ToList(), _pemodel.Format ?? new());
			else pfad = PDFGenerator.SchuelerBewertungPDF(new List<Schueler>(_pemodel.Schueler ?? new()), _pemodel.Format ?? new(), _pemodel.NeueSeiteProSchueler);
			webView.Source = new Uri(pfad);
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			_pmodel?.Navigate(_pmodel.History[^1], false);
		}
	}
}
