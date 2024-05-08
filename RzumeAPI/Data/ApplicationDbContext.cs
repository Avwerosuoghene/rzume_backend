using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RzumeAPI.Data.configuaration;
using RzumeAPI.Models;

namespace RzumeAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
             Userfile = Set<UserFile>();
             ApplicationUsers = Set<User>();
             Company = Set<Company>();
             Education = Set<Education>();
             Experience = Set<Experience>();
             Application = Set<Application>();
             Otp = Set<Otp>();
             Favorites = Set<Favorites>();
             Country = Set<Country>();


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<UserFile> Userfile { get; set; }

        public DbSet<Company> Company { get; set; }

        public DbSet<Education> Education { get; set; }

        public DbSet<Experience> Experience { get; set; }

        public DbSet<Application> Application { get; set; }

        public DbSet<Favorites> Favorites { get; set; }

        public DbSet<Otp> Otp { get; set; }
        public DbSet<Country> Country { get; set; }

    }
}

