using Microsoft.EntityFrameworkCore;
namespace SQLite
{
    public class LaufDBContext : DbContext
    {
        private static string _dbPath = "Datenbanken/EigeneDatenbank.db";
        public LaufDBContext()
        : base(GetDbContextOptions())
        {
            Directory.CreateDirectory("Datenbanken");
            Database.EnsureCreated();
           
        }
        public LaufDBContext(string _path)
       : base(GetDbContextOptions())
        {
            _dbPath = _path;
            Database.EnsureCreated();
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
