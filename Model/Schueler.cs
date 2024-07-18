namespace Klimalauf
{
    public class Schueler
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public Klasse Klasse { get; set; }
        public Geschlecht? Geschlecht { get; set; }
        public int Geburtsjahrgang { get; set; }

        public int KlasseId { get; set; }

        public virtual List<Runde> Runden { get; set; }

        public Schueler()
        {
        }

        public override string ToString()
        {
            return Vorname + " " + Nachname;
        }
    }
}
