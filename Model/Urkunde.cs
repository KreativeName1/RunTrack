namespace RunTrack
{
    public class Urkunde
    {
        public string LaufName { get; set; } = string.Empty;
        public string Kategorie { get; set; } = string.Empty;
        public string Auswertungsart { get; set; } = string.Empty;
        public string Wert { get; set; } = string.Empty;
        public List<string> _lstWerte { get; set; } = new List<string>();

        // Helper method to check if specific values should be shown
        public bool HasSpecificValues() => _lstWerte != null && _lstWerte.Count > 0;

        public string Platzierung { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Geschlecht { get; set; } = "Insgesamt";


        public Urkunde() { }

        public Urkunde(string laufName, string auswertungsart, string kategorie, string wert, List<string> lstWerte, string platzierung, string name, string geschlecht)
        {
            LaufName = laufName;
            Kategorie = kategorie;
            Auswertungsart = auswertungsart;
            Wert = wert;
            _lstWerte = lstWerte;
            Platzierung = platzierung;
            Name = name;
            Geschlecht = geschlecht;
        }
    }
}
