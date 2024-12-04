namespace RunTrack.Model
{
    public class Programm
    {
        public int Id { get; set; }
        private string _key;
        public string Key
        {
            get => _key;
            set => _key = value;
        }
        public List<Benutzer> Benutzer { get; set; }



    }
}
