namespace RunTrack
{
    // Definiert eine Klasse für Blattgrößen
    public class BlattGroesse
    {
        // Eindeutige ID für die Blattgröße
        public int Id { get; set; }

        // Name der Blattgröße, kann null sein
        public string? Name { get; set; }

        // Breite des Blattes
        public float Breite { get; set; }

        // Höhe des Blattes
        public float Hoehe { get; set; }

        // Konstruktor, der Breite und Höhe initialisiert
        public BlattGroesse(float breite, float hoehe)
        {
            this.Breite = breite;
            this.Hoehe = hoehe;
        }

        // Überladener Konstruktor, der Breite, Höhe und Name initialisiert
        public BlattGroesse(float breite, float hoehe, string name)
        {
            this.Breite = breite;
            this.Hoehe = hoehe;
            this.Name = name;
        }

        // Überschreibt die ToString-Methode, um den Namen oder eine Standardnachricht zurückzugeben
        public override string ToString()
        {
            return Name ?? "Nicht bekannte Größe";
        }
    }
}
