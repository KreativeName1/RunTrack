namespace RunTrack
{
    public class Runde
    {
        public int Id { get; set; }
        public DateTime Zeitstempel { get; set; }
        public Laeufer? Laeufer { get; set; }
        public int? LaeuferId { get; set; }
        public string? BenutzerName { get; set; }
        public string ProgrammKey { get; set; }

        public Runde()
        {

        }
    }
}
