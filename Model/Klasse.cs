namespace RunTrack
{
    public class Klasse
    {
        // Eindeutige ID der Klasse
        public int Id { get; set; }

        // Name der Klasse
        public string Name { get; set; } = string.Empty;

        // Schule, zu der die Klasse gehört
        public Schule Schule { get; set; }

        // ID der Schule, zu der die Klasse gehört
        public int SchuleId { get; set; }

        // Liste der Schüler in der Klasse
        public List<Schueler> Schueler { get; set; }

        // Art der Runden, die die Klasse läuft
        public RundenArt RundenArt { get; set; }

        // ID der RundenArt
        public int RundenArtId { get; set; }

        // Standardkonstruktor
        public Klasse()
        {
        }

        // Überschreibt die ToString-Methode, um den Namen der Klasse zurückzugeben
        public override string ToString()
        {
            return Name;
        }
    }
}
