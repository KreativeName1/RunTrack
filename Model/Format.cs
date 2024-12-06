namespace RunTrack
{

    // Definiert verschiedene Schriftarten
    public enum SchriftTyp
    {
        Normal,
        Fett,
        Kursiv,
        FettKursiv
    }
    // Definiert die Orientierung des Blattes
    public enum Orientierung
    {
        Hochformat,
        Querformat
    }

    // Klasse zur Definition des Formats
    public class Format
    {
        // Eindeutige ID des Formats
        public int Id { get; set; }

        // Name des Formats
        public string Name { get; set; } = string.Empty;

        // Seitenränder
        public float SeitenRandOben { get; set; } = 20;
        public float SeitenRandUnten { get; set; } = 20;
        public float SeitenRandLinks { get; set; } = 20;
        public float SeitenRandRechts { get; set; } = 20;

        // Zellenabstände und -größen
        public int ZellenAbstandHorizontal { get; set; } = 20;
        public int ZellenAbstandVertikal { get; set; } = 0;
        public int ZellenBreite { get; set; } = 150;
        public int ZellenHoehe { get; set; } = 50;

        // Schriftgröße und -typ
        public float SchriftGroesse { get; set; } = 12;
        public SchriftTyp SchriftTyp { get; set; } = SchriftTyp.Normal;

        // Blattgröße
        public BlattGroesse? BlattGroesse { get; set; }
        public int BlattGroesseId { get; set; }

        // Orientierung des Blattes
        public Orientierung Orientierung { get; set; } = Orientierung.Hochformat;

        // Weitere Einstellungen
        public bool KopfAnzeigen { get; set; } = true;
        public bool Zentriert { get; set; } = true;
        public int SpaltenAnzahl { get; set; } = 3;
        public double ZeilenAbstand { get; set; } = 1;

        // Konstruktor
        public Format()
        {
        }

        // Überschreibt die ToString-Methode, um den Namen des Formats zurückzugeben
        public override string ToString()
        {
            return Name;
        }
    }
}
