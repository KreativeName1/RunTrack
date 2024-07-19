using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Reflection;

namespace Klimalauf
{
    internal class ImportIntoDB
    {
        private ImportModel? _imodel;
        public ImportIntoDB(ImportModel imodel)
        {
            _imodel = imodel;

            // falls id 0 ist, mit name neu erstellen
            using (LaufDBContext db = new LaufDBContext())
            {
                if (_imodel.Schule.Id == 0)
                {
                    Schule schule = new Schule { Name = _imodel.NeuSchuleName };
                    db.Schulen.Add(schule);
                    db.SaveChanges();
                    _imodel.Schule = schule;
                }
                else
                {
                    _imodel.Schule = db.Schulen.Find(_imodel.Schule.Id) ?? throw new ImportException("Schule nicht gefunden");
                }

                // Klassen erstellen
                foreach (KlasseItem item in _imodel.KlasseItems)
                {
                    Klasse klasse = new Klasse { Name = item.Bezeichnung, Schule = _imodel.Schule, RundenArt = db.RundenArten.Find(item.RundenArt.Id ) };
                    if (klasse.RundenArt == null) throw new ImportException("Rundenart nicht gefunden");
                    db.Klassen.Add(klasse);
                    db.SaveChanges();
                }

                // Schüler erstellen
                foreach (object item in _imodel.CSVListe)
                {
                    Schueler schueler = new Schueler();
                    foreach (string property in _imodel.Reihenfolge)
                    {
                        int valueIndex = _imodel.Reihenfolge.IndexOf(property);
                        if (valueIndex >= 0)
                        {
                            string val = item.GetType().GetProperty("Spalte" + (valueIndex + 1)).GetValue(item).ToString();
                            switch (property)
                            {
                                case "Vorname":
                                    schueler.Vorname = val;
                                    break;
                                case "Nachname":
                                    schueler.Nachname = val;
                                    break;
                                case "Geschlecht":
                                    schueler.Geschlecht = val == "M" ? Geschlecht.Maennlich : Geschlecht.Weiblich;
                                    break;
                                case "Geburtsjahrgang":
                                    schueler.Geburtsjahrgang = int.Parse(val);
                                    break;
                                case "Klasse":
                                    schueler.Klasse = db.Klassen.First(k => k.Name == val);
                                    break;
                            }
                        }
                    }
                    db.Schueler.Add(schueler);
                    db.SaveChanges();
                }


            }
        }
    }
}
