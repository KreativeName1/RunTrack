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
				SeedBlattgroessen();
			}
		}

		public LaufDBContext(string _path)
				: base(GetDbContextOptions())
		{
			_dbPath = _path;
			Database.EnsureCreated();
		}
		public void SeedBlattgroessen()
		{
			List<BlattGroesse> blattGroessen = new List<BlattGroesse>();
			blattGroessen.Add(new (2384f, 3370f, "A0"));
			blattGroessen.Add(new (1684f, 2384f, "A1"));
			blattGroessen.Add(new (1190f, 1684f, "A2"));
			blattGroessen.Add(new (842f, 1190f, "A3"));
			blattGroessen.Add(new (595f, 842f, "A4"));
			blattGroessen.Add(new (420f, 595f, "A5"));
			blattGroessen.Add(new (298f, 420f, "A6"));
			blattGroessen.Add(new (210f, 298f, "A7"));
			blattGroessen.Add(new (148f, 210f, "A8"));
			blattGroessen.Add(new (105f, 148f, "A9"));
			blattGroessen.Add(new (74f, 105f, "A10"));
			blattGroessen.Add(new (2835f, 4008f, "B0"));
			blattGroessen.Add(new (2004f, 2835f, "B1"));
			blattGroessen.Add(new (1417f, 2004f, "B2"));
			blattGroessen.Add(new (1001f, 1417f, "B3"));
			blattGroessen.Add(new (709f, 1001f, "B4"));
			blattGroessen.Add(new (504f, 709f, "B5"));
			blattGroessen.Add(new (358f, 504f, "B6"));
			blattGroessen.Add(new (252f, 358f, "B7"));
			blattGroessen.Add(new (179f, 252f, "B8"));
			blattGroessen.Add(new (127f, 179f, "B9"));
			blattGroessen.Add(new (90f, 127f, "B10"));
			blattGroessen.Add(new (850f, 1400f, "LEGAL"));
			blattGroessen.Add(new (1100f, 1700f, "LEDGER"));
			blattGroessen.Add(new (725f, 1050f, "Executive"));
			blattGroessen.Add(new (850f, 1100f, "Letter"));
			blattGroessen.Add(new (1100f, 1700f, "Tabloid"));

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
				Benutzer benutzer = new Benutzer();
				benutzer.Vorname = "Sascha";
				benutzer.Nachname = "Dierl";

				Schule schule1 = new Schule { Name = "BSZ Wiesau" };
				db.Schulen.Add(schule1);
				db.SaveChanges();

				Schule schule2 = new Schule { Name = "Mittelschule Wiesau" };
				db.Schulen.Add(schule2);
				db.SaveChanges();

				RundenArt rundenArt = new RundenArt { Name = "Kurze Runde", LaengeInMeter = 800 };
				db.RundenArten.Add(rundenArt);
				db.SaveChanges();

				RundenArt rundenArt2 = new RundenArt { Name = "Lange Runde", LaengeInMeter = 1300 };
				db.RundenArten.Add(rundenArt2);
				db.SaveChanges();

				Klasse klasse1 = new Klasse { Name = "BFI10A", Schule = schule1, RundenArt = rundenArt, Jahrgang = 10 };
				db.Klassen.Add(klasse1);
				db.SaveChanges();

				Klasse klasse2 = new Klasse { Name = "BFI11A", Schule = schule1, RundenArt = rundenArt2, Jahrgang = 11 };
				db.Klassen.Add(klasse2);
				db.SaveChanges();

				for (int i = 0; i < 20; i++)
				{
					Schueler schueler = new Schueler
					{
						Vorname = randomVorname(),
						Nachname = randomNachname(),
						Klasse = klasse1,
						Geschlecht = Geschlecht.Maennlich
					};
					db.Schueler.Add(schueler);
					db.SaveChanges();
				}

				for (int i = 0; i < 20; i++)
				{
					Schueler schueler = new Schueler
					{
						Vorname = randomVorname(),
						Nachname = randomNachname(),
						Klasse = klasse2,
						Geschlecht = Geschlecht.Maennlich
					};
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
						Runde runde = new Runde
						{
							Schueler = schueler,
							BenutzerName = benutzer.Vorname + " " + benutzer.Nachname,
							Zeitstempel = lastTimestamp.AddSeconds(random.Next(60, 1000))
						};
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
			return vorname[rnd.Next(vorname.Length)];
		}

		private string randomNachname()
		{
			string[] nachname = { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz" };
			Random rnd = new Random();
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
