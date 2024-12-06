namespace RunTrack
{
    // Definiert eine Klasse namens RundenArt
    public class RundenArt
    {
        // Eigenschaft für die eindeutige Identifikation
        public int Id { get; set; }

        // Eigenschaft für den Namen der RundenArt, standardmäßig ein leerer String
        public string Name { get; set; } = string.Empty;

        // Eigenschaft für die Länge der Runde in Metern, standardmäßig 1000 Meter
        public int LaengeInMeter { get; set; } = 1000;

        // Eigenschaft für das maximale Scan-Intervall in Sekunden, standardmäßig 60 Sekunden
        public int MaxScanIntervalInSekunden { get; set; } = 60;

        // Überschreibt die ToString-Methode, um den Namen der RundenArt zurückzugeben
        public override string ToString()
        {
            return Name;
        }
    }
}