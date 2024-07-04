using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    public class Schueler
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Klasse Klasse { get; set;  }

        public int KlasseId { get; set; }

        public virtual List<Runde> Runden { get; set; }

        public Schueler()
        {
        }
    }
}
