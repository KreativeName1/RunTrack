using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace RunTrack
{
    public class Schueler : Laeufer
    {
        public Klasse Klasse { get; set; }
        public int KlasseId { get; set; }

        public Schueler() { }
    }
}
