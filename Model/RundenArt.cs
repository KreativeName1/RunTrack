using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    public class RundenArt
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int LaengeInMeter { get; set; }
        public int MaxScanIntervalInSekunden { get; set; } = 60;
    }
}
