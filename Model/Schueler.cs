using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace RunTrack
{
    // Definiert eine Klasse namens Schueler, die von Laeufer erbt
    public class Schueler : Laeufer
    {
        // Eigenschaft für die Klasse des Schülers
        public Klasse Klasse { get; set; }

        // Fremdschlüssel für die Klasse
        public int KlasseId { get; set; }

        // Standardkonstruktor
        public Schueler() { }
    }
}
