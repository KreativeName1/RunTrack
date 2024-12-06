namespace RunTrack
{
    public class Runde
    {
        // Eindeutige ID der Runde
        public int Id { get; set; }

        // Zeitstempel der Runde
        public DateTime Zeitstempel { get; set; }

        // Läufer, der die Runde gelaufen ist (optional)
        public Laeufer? Laeufer { get; set; }

        // ID des Läufers (optional)
        public int? LaeuferId { get; set; }

        // Benutzername des Läufers (optional)
        public string? BenutzerName { get; set; }

        // Schlüssel des Programms
        public string ProgrammKey { get; set; }

        // Standardkonstruktor
        public Runde()
        {

        }
    }
}
