using Microsoft.EntityFrameworkCore;
using System.IO;
namespace Klimalauf
{
    public class LaufDBContext : DbContext
    {
        private static string _dbPath = "Dateien/EigeneDatenbank.db";
        public LaufDBContext()
        : base(GetDbContextOptions())
        {
            Directory.CreateDirectory("Dateien");

            if (!File.Exists(_dbPath) || new FileInfo(_dbPath).Length == 0)
            {
                Database.EnsureCreated();
                SeedTestData();
            }

        }
        public LaufDBContext(string _path)
       : base(GetDbContextOptions())
        {
            _dbPath = _path;
            Database.EnsureCreated();
        }

        public void SeedTestData()
        {
            using (var db = new LaufDBContext())
            {
                Benutzer benutzer = new Benutzer();
                benutzer.Vorname = "Sascha";
                benutzer.Nachname = "Dierl";

                Schule schule1 = new Schule();
                schule1.Name = "BSZ Wiesau";
                db.Schulen.Add(schule1);
                db.SaveChanges();

                Schule schule2 = new Schule();
                schule2.Name = "Mittelschule Wiesau";
                db.Schulen.Add(schule2);
                db.SaveChanges();



                RundenArt rundenArt = new RundenArt();
                rundenArt.Name = "Kurze Runde";
                rundenArt.LaengeInMeter = 800;
                db.RundenArten.Add(rundenArt);
                db.SaveChanges();

                RundenArt rundenArt2 = new RundenArt();
                rundenArt2.Name = "Lange Runde";
                rundenArt2.LaengeInMeter = 1300;
                db.RundenArten.Add(rundenArt2);
                db.SaveChanges();

                Klasse klasse1 = new Klasse();
                klasse1.Name = "BFI10A";
                klasse1.Schule = schule1;
                klasse1.RundenArt = rundenArt;
                klasse1.Jahrgang  = 10;
                db.Klassen.Add(klasse1);
                db.SaveChanges();

                Klasse klasse2 = new Klasse();
                klasse2.Name = "BFI11A";
                klasse2.Schule = schule1;
                klasse2.Jahrgang = 11;
                klasse2.RundenArt = rundenArt2;
                db.Klassen.Add(klasse2);
                db.SaveChanges();

                for (int i = 0; i < 20; i++)
                {
                    Schueler schueler = new Schueler();
                    schueler.Vorname = randomVorname();
                    schueler.Nachname =  randomNachname();
                    schueler.Klasse = db.Klassen.First();
                    schueler.Geschlecht = Geschlecht.Maennlich;
                    db.Schueler.Add(schueler);
                    db.SaveChanges();
                }

                for (int i = 0; i < 20; i++)
                {
                    Schueler schueler = new Schueler();
                    schueler.Klasse = klasse2;
                    schueler.Vorname = randomVorname();
                    schueler.Nachname = randomNachname();
                    schueler.Geschlecht = Geschlecht.Maennlich;
                    db.Schueler.Add(schueler);
                    db.SaveChanges();
                }

                foreach (Schueler schueler in db.Schueler)
                {
                    Random random = new Random();
                    int rounds = random.Next(8, 20);
                    DateTime lastTimestamp = DateTime.Now;
                    for (int i = 0; i < rounds; i++)
                    {
                        Runde runde = new Runde();
                        runde.Schueler = schueler;
                        runde.BenutzerName = benutzer.Vorname + " " + benutzer.Nachname;
                        lastTimestamp = lastTimestamp.AddSeconds(random.Next(60, 1000));
                        runde.Zeitstempel = lastTimestamp;
                        db.Runden.Add(runde);
                        db.SaveChanges();
                    }

                }
            }
        }

        private string randomVorname()
        {
            string[] vorname = { "Max", "Moritz", "Hans", "Peter", "Paul", "Klaus", "Karl", "Kurt", "Kai" };
            Random rnd = new Random();
            int index = rnd.Next(vorname.Length);
            return vorname[index];

        }

        private string randomNachname()
        {
            string[] nachname = { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz" };
            Random rnd = new Random();
            int index = rnd.Next(nachname.Length);
            return nachname[index];
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
         .HasForeignKey(k => k.SchuleId);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.Schueler)
                .WithOne(s => s.Klasse)
                .HasForeignKey(s => s.KlasseId);

            modelBuilder.Entity<Schueler>()
                .HasMany(s => s.Runden)
                .WithOne(r => r.Schueler)
                .HasForeignKey(r => r.SchuelerId);
        }


        public DbSet<Klasse> Klassen { get; set; }
        public DbSet<Schule> Schulen { get; set; }
        public DbSet<Schueler> Schueler { get; set; }
        public DbSet<Runde> Runden { get; set; }
        public DbSet<RundenArt> RundenArten { get; set; }


    }
}
