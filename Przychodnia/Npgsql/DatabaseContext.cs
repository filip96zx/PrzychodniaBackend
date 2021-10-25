using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Przychodnia.Models;
using Przychodnia.Npgsql.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Przychodnia.Npgsql
{
    public class DatabaseContext : DbContext
    { 
        public DbSet<User> Users { get; set; }
        public DbSet<IdentityUserClaim<int>> IdentityUserClaim { get; set; }
        public DbSet<IdentityUserRole<int>> UserRole { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<IdentityRoleClaim<int>> RoleClaim { get; set; }
        public DbSet<Visit> Visits { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new IdentityUserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new VisitConfiguration());
        }


    }
}