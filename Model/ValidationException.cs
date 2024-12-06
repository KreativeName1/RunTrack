using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunTrack
{
    // Definiert eine benutzerdefinierte Ausnahme (Exception) namens ValidationException
    public class ValidationException : Exception
    {
        // Konstruktor, der eine Nachricht als Parameter akzeptiert und diese an die Basisklasse Exception weitergibt
        public ValidationException(string message) : base(message)
        {
        }
    }
}
