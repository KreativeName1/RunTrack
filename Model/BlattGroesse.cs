namespace RunTrack
{
    public class BlattGroesse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public float Breite { get; set; }
        public float Hoehe { get; set; }

        public BlattGroesse(float breite, float hoehe)
        {
            this.Breite = breite;
            this.Hoehe = hoehe;
        }
        public BlattGroesse(float breite, float hoehe, string name)
        {
            this.Breite = breite;
            this.Hoehe = hoehe;
            this.Name = name;
        }

        public override string ToString()
        {
            return Name ?? "Nicht bekannte Größe";
        }
    }
}
