using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTrack
{
    public class Urkunde
    {
        public string LaufName { get; set; } = string.Empty;
        public string Kategorie { get; set; } = string.Empty;
        public string Auswertungsart { get; set; } = string.Empty;
        public string Wert { get; set; } = string.Empty;
        public string Platzierung { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;


        public Urkunde() { }

        public Urkunde(string laufName, string auswertungsart, string kategorie, string wert, string platzierung, string name )
        {
            LaufName = laufName;
            Kategorie = kategorie;
            Auswertungsart = auswertungsart;
            Wert = wert;
            Platzierung = platzierung;
            Name = name;
        }
    }
}
