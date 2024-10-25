using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTrack
{

    // NICHT FERTIG

    // Anstatt das nur Schüler laufen können, sollte es auch möglich sein, dass Lehrer und andere Personen laufen können.
    // Dafür gibt es nun die Klasse Laeufer, die die allgemeinen Eigenschaften eines Läufers enthält.
    // Der Schüler erbt von Laeufer und hat zusätzlich noch die Klasse, in der er ist.

    // Es muss noch das anlegen, scannen und die Auswertung von Runden implementiert werden.
    public class Laeufer
    {
        public int? Id { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public Geschlecht? Geschlecht { get; set; }
        public int Geburtsjahrgang { get; set; }
        public List<Runde> Runden { get; set; }

        // Wird nicht für Schüler benutzt, da diese in einer Klasse sind, die bereits eine RundenArt hat.
        public RundenArt? RundenArt { get; set; }
        public int? RundenArtId { get; set; }

    }
}
