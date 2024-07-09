using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    public class Format
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float SeitenRandOben { get; set; } = 0;
        public float SeitenRandUnten { get; set; } = 0;
        public float SeitenRandLinks { get; set; } = 0;
        public float SeitenRandRechts { get; set; } = 0;
        public int SpaltenAnzahl { get; set; } = 1;
        public int ZeilenAnzahl { get; set; } = 1;
        public int ZellenAbstandHorizontal { get; set; } = 0;
        public int ZellenAbstandVertikal { get; set; } = 0;
        public int ZellenBreite { get; set; } = 100;
        public int ZellenHoehe { get; set; } = 100;
        public string[] Blattgroessen { get; set; } = new string[] { "A4", "A5", "A6" };

        public bool HeaderAnzeigen { get; set; } = true;

        public Format(float seitenRandOben, float seitenRandUnten, float seitenRandLinks, float seitenRandRechts, int spaltenAnzahl, int zeilenAnzahl, int zellenAbstandHorizontal, int zellenAbstandVertikal, int zellenBreite, int zellenHoehe, string[] blattgroessen)
        {
            this.SeitenRandOben = seitenRandOben;
            this.SeitenRandUnten = seitenRandUnten;
            this.SeitenRandLinks = seitenRandLinks;
            this.SeitenRandRechts = seitenRandRechts;
            this.SpaltenAnzahl = spaltenAnzahl;
            this.ZeilenAnzahl = zeilenAnzahl;
            this.ZellenAbstandHorizontal = zellenAbstandHorizontal;
            this.ZellenAbstandVertikal = zellenAbstandVertikal;
            this.ZellenBreite = zellenBreite;
            this.ZellenHoehe = zellenHoehe;
            this.Blattgroessen = blattgroessen;
        }

        public Format()
        {

        }
    }
}
