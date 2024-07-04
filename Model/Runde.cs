using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    public class Runde
    {
        public int Id { get; set; }
        public DateTime Zeitstempel { get; set; }
        public Schueler Schueler { get; set; }
        public int SchuelerId { get; set; }
        public string Name { get; set; }

        public Runde()
        {
        }
    }
}
