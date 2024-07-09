using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klimalauf
{
    public class PDFEditorModel : BaseModel
    {
        private Format _format;

        private Klasse _klasse;

        public Format Format
        {
            get { return _format; }
            set
            {
                _format = value;
                OnPropertyChanged("Format");
            }
        }

        public Klasse Klasse
        {
            get { return _klasse; }
            set
            {
                _klasse = value;
                OnPropertyChanged("Klasse");
            }
        }


    }
}
