namespace RunTrack
{
    public class Urkunde
    {
        // Eigenschaften der Urkunde
        public string LaufName { get; set; } // Name des Laufs
        public string Auswertungsart { get; set; } // Art der Auswertung
        public string Worin { get; set; } // Kategorie oder Disziplin
        public string Wert { get; set; } // Wert der Leistung
        public string Platzierung { get; set; } // Platzierung des Teilnehmers
        public string Name { get; set; } // Name des Teilnehmers
        public string Geschlecht { get; set; } // Geschlecht des Teilnehmers
        public TimeSpan SchnellsteRunde { get; set; } // Zeit der schnellsten Runde
        public int AnzahlRunden { get; set; } // Anzahl der gelaufenen Runden
        public int GelaufeneMeter { get; set; } // Anzahl der gelaufenen Meter
        public string Rundenart { get; set; } // Art der Runden (z.B. 400m, 800m)

        // Konstruktor zur Initialisierung der Eigenschaften
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
            Rundenart = rundenart;
        }
    }


}
