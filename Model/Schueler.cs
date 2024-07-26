namespace RunTrack
{
    public class Schueler
    {
        public int Id { get; set; }
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public Klasse Klasse { get; set; }
        public Geschlecht? Geschlecht { get; set; }
        public int Geburtsjahrgang { get; set; }

        public int KlasseId { get; set; }

        public virtual List<Runde> Runden { get; set; }

        public Schueler()
        {
            Runden = new List<Runde>();
            Klasse = new Klasse();
        }

        public override string ToString()
        {
            return Vorname + " " + Nachname;
        }
    }
}
