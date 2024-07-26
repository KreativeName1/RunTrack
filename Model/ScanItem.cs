namespace RunTrack
{
    public class ScanItem
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Schule { get; set; } = string.Empty;

        public ScanItem(int id, string name, string schule)
        {
            Id = id;
            Name = name;
            Schule = schule;
        }

        public ScanItem() { }

        public ScanItem(int id)
        {
            Id = id;
        }

        public static List<ScanItem>? AlleLesen()
        {
            return null;
        }
    }

}
