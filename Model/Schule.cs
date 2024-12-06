namespace RunTrack
{
    // Definiert eine Klasse namens Schule
    public class Schule
    {
        // Eigenschaft für die eindeutige Identifikation der Schule
        public int Id { get; set; }

        // Eigenschaft für den Namen der Schule, standardmäßig ein leerer String
        public string Name { get; set; } = string.Empty;

        // Liste der Klassen, die zu dieser Schule gehören
        public List<Klasse> Klassen { get; set; } = new List<Klasse>();

        // Standardkonstruktor
        public Schule()
        {
        }

        // Überschreibt die ToString-Methode, um den Namen der Schule zurückzugeben
        public override string ToString()
        {
            return Name;
        }
    }
}
