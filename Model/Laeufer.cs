namespace RunTrack
{
    // Definiert die Klasse Laeufer
    public class Laeufer
    {
        // Eindeutige ID des Läufers
        public int Id { get; set; }

        // Vorname des Läufers
        public string Vorname { get; set; }

        // Nachname des Läufers
        public string Nachname { get; set; }

        // Geschlecht des Läufers (optional)
        public Geschlecht? Geschlecht { get; set; }

        // Geburtsjahrgang des Läufers mit Validierung
        public int Geburtsjahrgang
        {
            get { return _geburtsjahrgang; }
            set
            {
                // Setzt den Geburtsjahrgang auf 1900, wenn der Wert kleiner als 1900 ist
                if (value < 1900)
                    _geburtsjahrgang = 1900;
                else
                    _geburtsjahrgang = value;
            }
        }
        // Private Variable für den Geburtsjahrgang
        private int _geburtsjahrgang;

        // Liste der Runden, die der Läufer absolviert hat
        public List<Runde> Runden { get; set; }

        // Art der Runden (optional)
        public RundenArt? RundenArt { get; set; }

        // ID der RundenArt (optional)
        public int? RundenArtId { get; set; }
    }
}
