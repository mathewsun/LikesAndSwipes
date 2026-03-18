using LikesAndSwipes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LikesAndSwipes.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<LocationEntity> Locations => Set<LocationEntity>();

        public DbSet<Interests> Interests => Set<Interests>();

        public DbSet<UserInterests> UserInterests => Set<UserInterests>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.IsNpgsql())
            {
                modelBuilder.HasPostgresExtension("postgis");
            }

            base.OnModelCreating(modelBuilder);

            var createdDefaultValueSql = Database.IsNpgsql() ? "now()" : "CURRENT_TIMESTAMP";

            modelBuilder
               .Entity<User>()
               .Property(e => e.Created)
               .HasDefaultValueSql(createdDefaultValueSql);

            modelBuilder
               .Entity<LocationEntity>()
               .Property(e => e.Created)
               .HasDefaultValueSql(createdDefaultValueSql);

            modelBuilder
               .Entity<Interests>()
               .Property(e => e.Created)
               .HasDefaultValueSql(createdDefaultValueSql);

            modelBuilder
               .Entity<UserInterests>()
               .Property(e => e.Created)
               .HasDefaultValueSql(createdDefaultValueSql);
        }
    }
}
