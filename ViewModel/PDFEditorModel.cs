﻿using System.Collections.ObjectModel;

namespace RunTrack
{
    public class PDFEditorModel : BaseModel
    {
        private Format? _format;

        private Klasse? _klasse;
        private ObservableCollection<Schueler>? _schueler;

        private bool _neueSeiteProSchueler = true;

        private ObservableCollection<object>? _liste;

        private ObservableCollection<Urkunde>? _urkunden;

        public Format? Format
        {
            get { return _format; }
            set
            {
                _format = value;
                OnPropertyChanged("Format");
            }
        }



        public Klasse? Klasse
        {
            get { return _klasse; }
            set
            {
                _klasse = value;
                OnPropertyChanged("Klasse");
            }
        }
        public ObservableCollection<Schueler>? Schueler
        {
            get { return _schueler; }
            set
            {
                _schueler = value;
                OnPropertyChanged("Schueler");
            }
        }

        public ObservableCollection<object>? Liste
        {
            get { return _liste; }
            set
            {
                _liste = value;
                OnPropertyChanged("Liste");
            }
        }

        public ObservableCollection<Urkunde>? Urkunden
        {
            get { return _urkunden; }
            set
            {
                _urkunden = value;
                OnPropertyChanged("Urkunden");
            }
        }


        public bool NeueSeiteProSchueler
        {
            get { return _neueSeiteProSchueler; }
            set
            {
                _neueSeiteProSchueler = value;
                OnPropertyChanged("NeueSeiteProSchueler");
            }
        }

    }
}
