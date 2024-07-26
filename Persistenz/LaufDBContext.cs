using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;

namespace Klimalauf
{
    public class LaufDBContext : DbContext
    {
        private static string _dbPath = "./Dateien/EigeneDatenbank.db";
        public LaufDBContext()
              : base(GetDbContextOptions())
        {

            Directory.CreateDirectory("./Dateien");


            if (!File.Exists(_dbPath) || new FileInfo(_dbPath).Length == 0)
            {
                Database.EnsureCreated();
                SeedTestData();
                SeedBlattgroessen();
            }
            Trace.WriteLine(Path.GetFullPath(_dbPath));
        }

        public LaufDBContext(string _path)
              : base(GetDbContextOptions())
        {
            _dbPath = _path;
            Database.EnsureCreated();
        }
        public void SeedBlattgroessen()
        {
            List<BlattGroesse> blattGroessen = new()
            {
                new(2384f, 3370f, "A0"),
                new(1684f, 2384f, "A1"),
                new(1190f, 1684f, "A2"),
                new(842f, 1190f, "A3"),
                new(595f, 842f, "A4"),
                new(420f, 595f, "A5"),
                new(298f, 420f, "A6"),
                new(210f, 298f, "A7"),
                new(148f, 210f, "A8"),
                new(105f, 148f, "A9"),
                new(74f, 105f, "A10"),
                new(2835f, 4008f, "B0"),
                new(2004f, 2835f, "B1"),
                new(1417f, 2004f, "B2"),
                new(1001f, 1417f, "B3"),
                new(709f, 1001f, "B4"),
                new(504f, 709f, "B5"),
                new(358f, 504f, "B6"),
                new(252f, 358f, "B7"),
                new(179f, 252f, "B8"),
                new(127f, 179f, "B9"),
                new(90f, 127f, "B10"),
                new(850f, 1400f, "LEGAL"),
                new(1100f, 1700f, "LEDGER"),
                new(725f, 1050f, "Executive"),
                new(850f, 1100f, "Letter"),
                new(1100f, 1700f, "Tabloid")
            };

            using (var db = new LaufDBContext())
            {
                foreach (BlattGroesse bg in blattGroessen)
                {
                    if (!db.BlattGroessen.Any(b => b.Name == bg.Name))
                    {
                        db.BlattGroessen.Add(bg);
                    }
                }
                db.SaveChanges();
            }
        }

        public void SeedTestData()
        {
            using (var db = new LaufDBContext())
            {
                Benutzer benutzer = new();
                benutzer.Vorname = "Sascha";
                benutzer.Nachname = "Dierl";

                Schule schule1 = new() { Name = "BSZ Wiesau" };
                db.Schulen.Add(schule1);
                db.SaveChanges();

                Schule schule2 = new() { Name = "Mittelschule Wiesau" };
                db.Schulen.Add(schule2);
                db.SaveChanges();

                RundenArt rundenArt = new() { Name = "Kurze Runde", LaengeInMeter = 800 };
                db.RundenArten.Add(rundenArt);
                db.SaveChanges();

                RundenArt rundenArt2 = new() { Name = "Lange Runde", LaengeInMeter = 1300 };
                db.RundenArten.Add(rundenArt2);
                db.SaveChanges();

                Klasse klasse1 = new() { Name = "BFI10A", Schule = schule1, RundenArt = rundenArt };
                db.Klassen.Add(klasse1);
                db.SaveChanges();

                Klasse klasse2 = new() { Name = "BFI11A", Schule = schule1, RundenArt = rundenArt2 };
                db.Klassen.Add(klasse2);
                db.SaveChanges();
                Random rnd = new();
                for (int i = 0; i < 20; i++)
                {
                    Schueler schueler = new()
                    {
                        Vorname = RandomVorname(),
                        Nachname = RandomNachname(),
                        Klasse = klasse1,
                        Geburtsjahrgang = rnd.Next(1995, 2010),
                        Geschlecht = Geschlecht.Maennlich
                    };
                    db.Schueler.Add(schueler);
                    db.SaveChanges();
                }

                for (int i = 0; i < 20; i++)
                {
                    Schueler schueler = new()
                    {
                        Vorname = RandomVorname(),
                        Nachname = RandomNachname(),
                        Klasse = klasse2,
                        Geburtsjahrgang = rnd.Next(1995, 2010),
                        Geschlecht = Geschlecht.Maennlich
                    };
                    db.Schueler.Add(schueler);
                    db.SaveChanges();
                }
            }
        }

        private string RandomVorname()
        {
            string[] vorname = { "Max", "Moritz", "Hans", "Peter", "Paul", "Klaus", "Karl", "Kurt", "Kai" };
            Random rnd = new();
            return vorname[rnd.Next(vorname.Length)];
        }

        private string RandomNachname()
        {
            string[] nachname = { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz" };
            Random rnd = new();
            return nachname[rnd.Next(nachname.Length)];
        }

        private static DbContextOptions GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<LaufDBContext>();
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
            optionsBuilder.EnableSensitiveDataLogging();
            return optionsBuilder.Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schule>()
                  .HasMany(s => s.Klassen)
                  .WithOne(k => k.Schule)
                  .HasForeignKey(k => k.SchuleId)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Klasse>()
                  .HasMany(k => k.Schueler)
                  .WithOne(s => s.Klasse)
                  .HasForeignKey(s => s.KlasseId)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Schueler>()
                  .HasMany(s => s.Runden)
                  .WithOne(r => r.Schueler)
                  .HasForeignKey(r => r.SchuelerId)
                  .OnDelete(DeleteBehavior.Cascade);

            // format has one BlattGroesse
            modelBuilder.Entity<Format>()
                  .HasOne(f => f.BlattGroesse)
                  .WithMany()
                  .HasForeignKey(f => f.BlattGroesseId)
                  .OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<Klasse> Klassen { get; set; }
        public DbSet<Schule> Schulen { get; set; }
        public DbSet<Schueler> Schueler { get; set; }
        public DbSet<Runde> Runden { get; set; }
        public DbSet<RundenArt> RundenArten { get; set; }
        public DbSet<Benutzer> Benutzer { get; set; }
        public DbSet<Format> Formate { get; set; }
        public DbSet<BlattGroesse> BlattGroessen { get; set; }
    }
}
