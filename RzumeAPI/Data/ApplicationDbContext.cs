using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Models;

namespace RzumeAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Application>()
                .HasOne(x => x.User)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id)
                .IsRequired();


            modelBuilder.Entity<Favorites>()
              .HasMany(x => x.Applications)
              .WithOne(x => x.Favorites)
              .HasForeignKey(x => x.ApplicationID)
              .HasPrincipalKey(x => x.FavoritesID)
              .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne(x => x.Favorites)
                .WithOne(x => x.User)
                .HasForeignKey<Favorites>(u => u.UserId);

            modelBuilder.Entity<Education>()
           .HasOne(x => x.User)
           .WithMany(u => u.Education)
           .HasForeignKey(c => c.UserId)
           .IsRequired();

            modelBuilder.Entity<Experience>()
           .HasOne(x => x.User)
           .WithMany(u => u.Experience)
           .HasForeignKey(c => c.UserId)
           .IsRequired();




        }

        public DbSet<User> ApplicationUsers { get; set; }

        public DbSet<Company> Company { get; set; }

        public DbSet<Education> Education { get; set; }

        public DbSet<Experience> Experience { get; set; }

        public DbSet<Application> Application { get; set; }

        public DbSet<Favorites> Favorites { get; set; }

        public DbSet<Otp> Otp { get; set; }

    }
}

