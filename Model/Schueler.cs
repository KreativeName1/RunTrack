namespace RunTrack
{
    public class Schueler : Laeufer
    {
        public Klasse Klasse { get; set; }

        public int KlasseId { get; set; }



        public Schueler()
        {
            Klasse = new Klasse();
        }

        public override string ToString()
        {
            return Vorname + " " + Nachname;
        }
    }
}
