
namespace RunTrack
{
    public class Benutzer
    {
        public int Id { get; set; }
        public string Passwort { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public bool IsAdmin { get; set; } = false;

        public Benutzer()
        {
            Id = 0;
            Passwort = "";
            Vorname = "";
            Nachname = "";
        }

        public string Benutzername
        {
            get
            {
                if (string.IsNullOrEmpty(Vorname) && string.IsNullOrEmpty(Nachname)) return "";
                return $"{Vorname}, {Nachname}";
            }
        }
    }
}
