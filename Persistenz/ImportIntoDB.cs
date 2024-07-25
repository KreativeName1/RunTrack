using iText.StyledXmlParser.Node;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Reflection;

namespace Klimalauf
{
    internal class ImportIntoDB
    {
        private ImportModel _imodel;
        public ImportIntoDB(ImportModel imodel)
        {
            _imodel = imodel ?? throw new ImportException("Fehler beim Importieren");

            using (LaufDBContext db = new LaufDBContext())
            {

                if (_imodel.Schule == null) throw new ImportException("Schule nicht gefunden");
                if (_imodel.Schule.Id == 0)
                {
                    Schule schule = new Schule { Name = _imodel.NeuSchuleName ?? string.Empty };
                    db.Schulen.Add(schule);
                    _imodel.Schule = schule;
                }
                else
                {
                    _imodel.Schule = db.Schulen.Find(_imodel.Schule.Id) ?? throw new ImportException("Schule nicht gefunden");
                }

                // Klassen erstellen
                foreach (KlasseItem item in _imodel.KlasseItems)
                {
                    if (item.Bezeichnung == null && item.RundenArt != null) throw new ImportException("Klassenname darf nicht leer sein");
                    if (item.RundenArt == null && item.Bezeichnung != null) throw new ImportException("Rundenart darf nicht leer sein");
                    if (item.Bezeichnung == null && item.RundenArt == null) continue;
                    Klasse klasse = new Klasse { Name = item.Bezeichnung ?? string.Empty, Schule = _imodel.Schule, RundenArt = db.RundenArten.Find(item.RundenArt?.Id ) ?? new() };
                    if (klasse.RundenArt == null) throw new ImportException("Rundenart nicht gefunden");
                    db.Klassen.Add(klasse);
                }

                db.SaveChanges();
                try
                {
                    // Schüler erstellen
                    foreach (object item in _imodel.CSVListe)
                    {
                        Schueler schueler = new Schueler();
                        foreach (string property in _imodel.Reihenfolge)
                        {
                            int valueIndex = _imodel.Reihenfolge.IndexOf(property);
                            if (valueIndex >= 0)
                            {
                                switch (property)
                                {
                                    case "Vorname":
                                        schueler.Vorname = item.GetType().GetProperty("Spalte" + (valueIndex + 1))?.GetValue(item)?.ToString() ?? string.Empty;
                                        break;
                                    case "Nachname":
                                        schueler.Nachname = item.GetType().GetProperty("Spalte" + (valueIndex + 1))?.GetValue(item)?.ToString() ?? string.Empty;
                                        break;
                                    case "Geschlecht":
                                        schueler.Geschlecht = (item.GetType().GetProperty("Spalte" + (valueIndex + 1))?.GetValue(item)?.ToString() ?? "") == "M" ? Geschlecht.Maennlich : Geschlecht.Weiblich;
                                        break;
                                    case "Geburtsjahrgang":
                                        schueler.Geburtsjahrgang = int.Parse(item.GetType().GetProperty("Spalte" + (valueIndex + 1))?.GetValue(item)?.ToString() ?? string.Empty);
                                        break;
                                    case "Klasse":
                                        string name = item.GetType().GetProperty("Spalte" + (valueIndex + 1))?.GetValue(item)?.ToString() ?? string.Empty;
                                        schueler.Klasse = db.Klassen.First(k => k.Name == name && k.Schule.Id == _imodel.Schule.Id);
                                        break;
                                }
                            }
                        }
                        db.Schueler.Add(schueler);
                    }
                }
                catch (Exception e)
                {
                    foreach (Klasse klasse in db.Klassen.Where(k => k.Schule.Id == _imodel.Schule.Id))
                    {
                        db.Klassen.Remove(klasse);
                    }
                    db.Schulen.Remove(_imodel.Schule);

                    db.SaveChanges();
                    throw;
                }
                db.SaveChanges();


            }
        }
    }
}
