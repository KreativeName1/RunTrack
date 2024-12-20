using Microsoft.EntityFrameworkCore;
using TracerFile;
using TraceLevel = TracerFile.TraceLevel;

namespace RunTrack
{
    // Definiert die MergedDBContext-Klasse, die von DbContext erbt
    public class MergedDBContext : DbContext
    {
        // Definiert den Pfad zur internen Datenbank
        private static string _internalDBPath = MainModel.BaseFolder + "/Temp/internal.db";

        // Initialisiert einen Tracer für das Logging
        private Tracer Tracer = new("LOG_MergedDBContext.txt");

        // Konstruktor, der eine Liste von Datenbanken als Parameter nimmt
        public MergedDBContext(string[] databases)
        : base(GetDbContextOptions())
        {

            // Ordner für die temporäre Datenbank erstellen
            if (!System.IO.Directory.Exists(MainModel.BaseFolder + "/Temp")) System.IO.Directory.CreateDirectory(MainModel.BaseFolder + "/Temp");

            // Löscht und erstellt die Datenbank neu
            Database.EnsureDeleted();
            Database.EnsureCreated();

            // Lädt die Daten aus der internen Datenbank
            using (LaufDBContext thisDB = new())
            {
                RundenArten?.AddRange(thisDB.RundenArten.ToList());
                Schulen?.AddRange(thisDB.Schulen.ToList());
                Klassen?.AddRange(thisDB.Klassen.ToList());
                Schueler?.AddRange(thisDB.Schueler.ToList());
                Laeufer?.AddRange(thisDB.Laeufer.ToList());
                SaveChanges();
            }

            // Fügt die Runden aus den externen Datenbanken hinzu
            foreach (string db_path in databases)
            {
                using (LaufDBContext db = new(db_path))
                {
                    foreach (Runde runde in db.Runden)
                    {
                        runde.Id = 0;
                        Runden?.Add(runde);
                    }
                }
                SaveChanges();
            }

            // Initialisiert das maximale Scan-Intervall
            int MaxScanIntervalInSekunden = 0;

            // Erstellt ein Dictionary für die Runden und deren Intervalle
            Dictionary<List<Runde>, TimeSpan> RundenListe = new();

            // Durchläuft alle Läufer und fügt deren Runden zum Dictionary hinzu
            foreach (Laeufer l in Laeufer)
            {
                if (l is Schueler schueler) MaxScanIntervalInSekunden = schueler.Klasse.RundenArt.MaxScanIntervalInSekunden;
                else if (l is Laeufer laeufer) MaxScanIntervalInSekunden = laeufer.RundenArt.MaxScanIntervalInSekunden;

                if (l.Runden == null) continue;

                if (!RundenListe.TryGetValue(l.Runden, out TimeSpan intervall))
                {
                    RundenListe.Add(l.Runden, TimeSpan.FromSeconds(MaxScanIntervalInSekunden));
                }
            }

            // Entfernt alle Runden und fügt die bereinigten Runden hinzu
            Runden.RemoveRange(Runden);
            Runden.AddRange(EntferneZuNaheRunden(RundenListe));

            // Loggt die Erstellung des MergedDBContext
            Tracer.Trace($"MergedDBContext created", TraceLevel.Info);
        }

        // Methode zum Entfernen von zu nahen Runden
        public static List<Runde>? EntferneZuNaheRunden(Dictionary<List<Runde>, TimeSpan> dic)
        {
            List<Runde> bereinigteRunden = new();

            if (dic == null) return bereinigteRunden;

            foreach (KeyValuePair<List<Runde>, TimeSpan> item in dic)
            {
                List<Runde> runden = item.Key;
                TimeSpan intervall = item.Value;
                // Sortiert die Runden nach Zeitstempel
                runden.Sort((r1, r2) => r1.Zeitstempel.CompareTo(r2.Zeitstempel));
                // Initialisiert die letzte Runde und den letzten Läufer
                Runde letzteRunde = null;
                int? letzterLaeuferId = null;
                foreach (Runde runde in runden)
                {
                    if (letzteRunde == null ||
                        runde.Zeitstempel - letzteRunde.Zeitstempel >= intervall ||
                        runde.LaeuferId != letzterLaeuferId)
                    {
                        // Fügt die Runde hinzu, wenn der Abstand groß genug ist oder der Läufer gewechselt hat
                        bereinigteRunden.Add(runde);
                        letzteRunde = runde;
                        letzterLaeuferId = runde.LaeuferId;
                    }
                }
            }

            return bereinigteRunden;
        }

        // Methode zum Erstellen der DbContext-Optionen
        private static DbContextOptions GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MergedDBContext>();
            optionsBuilder.UseSqlite($"Data Source={_internalDBPath};Pooling=False");
            optionsBuilder.EnableSensitiveDataLogging();
            return optionsBuilder.Options;
        }

        // Konfiguriert die Modell-Erstellung
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schule>()
                .HasMany(s => s.Klassen)
                .WithOne(k => k.Schule)
                .HasForeignKey(k => k.SchuleId);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.Schueler)
                .WithOne(s => s.Klasse)
                .HasForeignKey(s => s.KlasseId);

            modelBuilder.Entity<Laeufer>()
                .HasMany(s => s.Runden)
                .WithOne(r => r.Laeufer)
                .HasForeignKey(r => r.LaeuferId);
        }

        // Überschreibt die Dispose-Methode und loggt die Entsorgung
        public override void Dispose()
        {
            Tracer.Trace("MergedDBContext disposed", TraceLevel.Info);
            base.Dispose();
        }

        // Überschreibt die DisposeAsync-Methode und loggt die Entsorgung
        public override ValueTask DisposeAsync()
        {
            Tracer.Trace("MergedDBContext disposed", TraceLevel.Info);
            return base.DisposeAsync();
        }

        // Definiert die DbSet-Eigenschaften für die verschiedenen Entitäten
        public DbSet<Klasse> Klassen { get; set; }
        public DbSet<Schule> Schulen { get; set; }
        public DbSet<Laeufer> Laeufer { get; set; }
        public DbSet<Schueler> Schueler { get; set; }
        public DbSet<Runde> Runden { get; set; }
        public DbSet<RundenArt> RundenArten { get; set; }
        public DbSet<Benutzer> Benutzer { get; set; }
    }
}
