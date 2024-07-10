using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    public class Benutzer
    {
        public int Id { get; set; }
        public string Passwort { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public bool IsAdmin { get; set; } = false;

        public Benutzer()
        {
        }
    }
}
