namespace RunTrack
{
    public class Urkunde
    {
        public string LaufName { get; set; }
        public string Auswertungsart { get; set; }
        public string Worin { get; set; }
        public string Wert { get; set; }
        public string Platzierung { get; set; }
        public string Name { get; set; }
        public string Geschlecht { get; set; }
        public TimeSpan SchnellsteRunde { get; set; }
        public int AnzahlRunden { get; set; }
        public int GelaufeneMeter { get; set; }
        public string Rundenart { get; set; } // Neue Eigenschaft

        public Urkunde(string laufName, string auswertungsart, string worin, string wert, string platzierung, string name, string geschlecht, TimeSpan schnellsteRunde, int anzahlRunden, int gelaufeneMeter, string rundenart)
        {
            LaufName = laufName;
            Auswertungsart = auswertungsart;
            Worin = worin;
            Wert = wert;
            Platzierung = platzierung;
            Name = name;
            Geschlecht = geschlecht;
            SchnellsteRunde = schnellsteRunde;
            AnzahlRunden = anzahlRunden;
            GelaufeneMeter = gelaufeneMeter;
            Rundenart = rundenart; // Zuweisung
        }
    }


}
