using Microsoft.EntityFrameworkCore;

namespace RunTrack
{
    public class MergedDBContext : DbContext
    {
        private static string _internalDBPath = "internal.db";
        public MergedDBContext(string[] databases)
        : base(GetDbContextOptions())
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            // Erst die Daten aus der internen Datenbank laden
            using (LaufDBContext thisDB = new())
            {
                RundenArten?.AddRange(thisDB.RundenArten.ToList());
                Schulen?.AddRange(thisDB.Schulen.ToList());
                Klassen?.AddRange(thisDB.Klassen.ToList());
                Schueler?.AddRange(thisDB.Schueler.ToList());
                Laeufer?.AddRange(thisDB.Laeufer.ToList());
                SaveChanges();
            }

            // Alle externen Datenbanken durchgehen und die Runden hinzufügen
            foreach (string db_path in databases)
            {
                using (LaufDBContext db = new(db_path))
                {
                    foreach (Runde runde in db.Runden)
                    {
                        runde.Id = 0;
                        Runden?.Add(runde);
                    }
                    SaveChanges();
                }
            }


            int MaxScanIntervalInSekunden = 0;

            Dictionary<List<Runde>, TimeSpan> RundenListe = new();

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
        }

        public static List<Runde>? EntferneZuNaheRunden(Dictionary<List<Runde>, TimeSpan> dic)
        {
            List<Runde> bereinigteRunden = new();

            if (dic == null) return bereinigteRunden;

            foreach (KeyValuePair<List<Runde>, TimeSpan> item in dic)
            {
                List<Runde> runden = item.Key;
                TimeSpan intervall = item.Value;
                // Sortiere die Runden nach Zeitstempel
                runden.Sort((r1, r2) => r1.Zeitstempel.CompareTo(r2.Zeitstempel));
                // Initialisiere die letzte Runde und den letzten Läufer
                Runde letzteRunde = null;
                int? letzterLaeuferId = null;
                foreach (Runde runde in runden)
                {
                    if (letzteRunde == null ||
                        runde.Zeitstempel - letzteRunde.Zeitstempel >= intervall ||
                        runde.LaeuferId != letzterLaeuferId)
                    {
                        // Abstand zwischen den Runden ist groß genug, es ist die erste Runde oder der Läufer hat sich gewechselt
                        bereinigteRunden.Add(runde);
                        letzteRunde = runde;
                        letzterLaeuferId = runde.LaeuferId;
                    }
                }
            }

            return bereinigteRunden;
        }


        private static DbContextOptions GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MergedDBContext>();
            optionsBuilder.UseSqlite($"Data Source={_internalDBPath}");
            optionsBuilder.EnableSensitiveDataLogging();
            return optionsBuilder.Options;
        }

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


        public DbSet<Klasse> Klassen { get; set; }
        public DbSet<Schule> Schulen { get; set; }
        public DbSet<Laeufer> Laeufer { get; set; }
        public DbSet<Schueler> Schueler { get; set; }
        public DbSet<Runde> Runden { get; set; }
        public DbSet<RundenArt> RundenArten { get; set; }
        public DbSet<Benutzer> Benutzer { get; set; }

    }
}
