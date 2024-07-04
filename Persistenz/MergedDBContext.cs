using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    public class MergedDBContext : DbContext
    {
        public MergedDBContext(string[] databases)
        : base(GetDbContextOptions())
        {
            Database.EnsureCreated();

            // get all data from the Runden table
            foreach (string db_path in databases)
            {
                using (var db = new LaufDBContext(db_path))
                {
                    foreach (var runde in db.Runden)
                    {
                        runde.Id = 0;
                        Schueler schueler;
                        List<Schueler> schuelerList;
                        using (var db2 = new LaufDBContext())
                        {
                            schuelerList = db2.Schueler.AsNoTracking().ToList();
                        }

                        runde.Schueler = schuelerList.FirstOrDefault(s => s.Id == runde.SchuelerId);
                        this.Runden.Add(runde);
                    }
                    this.SaveChanges();
                }
            }
        }

        private static DbContextOptions GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MergedDBContext>();
            optionsBuilder.UseSqlite($"Data Source=internal.db");
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
