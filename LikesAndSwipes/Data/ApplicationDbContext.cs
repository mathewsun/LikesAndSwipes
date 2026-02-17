using LikesAndSwipes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LikesAndSwipes.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<LocationEntity> Locations => Set<LocationEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            base.OnModelCreating(modelBuilder);

            modelBuilder
               .Entity<User>()
               .Property(e => e.Created)
               .HasDefaultValueSql("now()");

            modelBuilder
               .Entity<LocationEntity>()
               .Property(e => e.Created)
               .HasDefaultValueSql("now()");
        }
    }
}
