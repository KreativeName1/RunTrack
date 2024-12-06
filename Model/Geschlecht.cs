using System.ComponentModel;

namespace RunTrack
{
    // Definiert eine Enumeration für Geschlechter
    public enum Geschlecht
    {
        // Beschreibt das Geschlecht "Männlich"
        [Description("Männlich")]
        Maennlich,
        // Beschreibt das Geschlecht "Weiblich"
        [Description("Weiblich")]
        Weiblich,
        // Beschreibt das Geschlecht "Divers"
        [Description("Divers")]
        Divers
    }

    // Statische Hilfsklasse für die Geschlecht-Enumeration
    public static class GeschlechtHelper
    {
        // Methode, um die Beschreibung eines Geschlechts zu erhalten
        public static string GetDescription(Geschlecht geschlecht)
        {
            // Holt den Typ der Enumeration
            var type = geschlecht.GetType();
            // Holt die Member-Informationen des spezifischen Geschlechts
            var memInfo = type.GetMember(geschlecht.ToString());
            // Holt die Attribute des Members
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            // Gibt die Beschreibung des Attributs zurück
            return ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}