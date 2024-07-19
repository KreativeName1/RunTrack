using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    internal class ImportException : Exception
    { 
        public ImportException() { }

        public ImportException(string message) : base(message) { }
    }
}
