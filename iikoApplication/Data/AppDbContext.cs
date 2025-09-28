using iikoApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace iikoApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId);
                entity.HasIndex(e => e.ClientId).IsUnique();
                entity.HasIndex(e => e.SystemId).IsUnique();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SystemId)
                    .IsRequired()
                    .HasDefaultValueSql("gen_random_uuid()");
            });
        }
    }
}
