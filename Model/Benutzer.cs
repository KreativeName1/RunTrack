namespace RunTrack
{
    // Definiert die Klasse "Benutzer"
    public class Benutzer
    {
        // Eigenschaften der Klasse "Benutzer"
        public int Id { get; set; } // Benutzer-ID
        public string Passwort { get; set; } // Benutzerpasswort
        public string Vorname { get; set; } // Benutzer-Vorname
        public string Nachname { get; set; } // Benutzer-Nachname
        public bool IsAdmin { get; set; } = false; // Gibt an, ob der Benutzer ein Administrator ist

        // Konstruktor der Klasse "Benutzer"
        public Benutzer()
        {
            Id = 0;
            Passwort = "";
            Vorname = "";
            Nachname = "";
        }

        // Eigenschaft, die den Benutzernamen zurückgibt
        public string Benutzername
        {
            get
            {
                // Wenn Vorname und Nachname leer sind, gib einen leeren String zurück
                if (string.IsNullOrEmpty(Vorname) && string.IsNullOrEmpty(Nachname)) return "";
                // Andernfalls gib den formatierten Benutzernamen zurück
                return $"{Capitalize(Vorname)}, {Capitalize(Nachname)}";
            }
        }

        // Methode zur Kapitalisierung eines Strings
        private string Capitalize(string input)
        {
            // Wenn der Eingabestring leer ist, gib ihn zurück
            if (string.IsNullOrEmpty(input)) return input;
            // Andernfalls kapitalisiere den ersten Buchstaben und setze den Rest in Kleinbuchstaben
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }
    }
}
