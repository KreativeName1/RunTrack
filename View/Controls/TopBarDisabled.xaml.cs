using System.Windows.Controls;
using System.Windows.Input;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class TopBarDisabled : UserControl
    {
        // Private Variable für das MainModel
        private MainModel? _pmodel;

        // Konstruktor für die TopBar-Klasse
        public TopBarDisabled()
        {
            InitializeComponent();
            this.DataContext = this; // Setzt den DataContext auf die aktuelle Instanz
            _pmodel = FindResource("pmodel") as MainModel ?? new(); // Initialisiert _pmodel mit einer Ressource oder einer neuen Instanz
        }
    }
}
