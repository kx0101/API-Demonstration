using Microsoft.EntityFrameworkCore;

namespace apiprac
{
    public class ApplicationDBContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public ApplicationDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultSQLConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Villa>().Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
        }

        public DbSet<Villa> Villas { get; set; }

        public DbSet<VillaNumber> VillaNumbers { get; set; }

        public DbSet<LocalUser> Users { get; set; }
    }
}
