using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Klimalauf
{
    public class MergedDBContext : DbContext
    {
        private static string _internalDBPath = "internal.db";
        public MergedDBContext(string[] databases)
        : base(GetDbContextOptions())
        {            
            Database.EnsureCreated();
            
            // Erst die Daten aus der internen Datenbank laden
            using (var thisDB = new LaufDBContext())
            {
                RundenArten.AddRange(thisDB.RundenArten); 
                Schulen.AddRange(thisDB.Schulen);
                Klassen.AddRange(thisDB.Klassen);
                Schueler.AddRange(thisDB.Schueler);
                SaveChanges();
            }

            // Alle externen Datenbanken durchgehen und die Runden hinzufügen
            foreach (string db_path in databases)
            {
                using (var db = new LaufDBContext(db_path))
                {
                    foreach (var runde in db.Runden)
                    {
                        runde.Id = 0;
                        Runden.Add(runde);
                    }
                    SaveChanges();
                }
            }
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
