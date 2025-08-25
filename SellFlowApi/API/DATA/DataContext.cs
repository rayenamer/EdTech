using System;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using API.entities;

namespace API.Data;

public class DataContext(DbContextOptions options)
        : IdentityDbContext
        <
          AppUser,
          AppRole,
          int,
          IdentityUserClaim<int>,
          AppUserRole,
          IdentityUserLogin<int>,
          IdentityRoleClaim<int>,
          IdentityUserToken<int>
        >(options)
{
        public DbSet<UniProgram> UniPrograms { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<Application> Applications { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
                base.OnModelCreating(builder);

                builder.Entity<AppUser>()
                       .HasMany(ur => ur.UserRoles)
                       .WithOne(u => u.User)
                       .HasForeignKey(ur => ur.UserId)
                       .IsRequired();

                builder.Entity<AppRole>()
                        .HasMany(ur => ur.UserRoles)
                        .WithOne(u => u.Role)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                // AppUser ↔ Documents (1:Many)
                builder.Entity<AppUser>()
                        .HasMany(u => u.Documents)
                        .WithOne()  // Empty WithOne() since Document has no navigation property back to AppUser
                        .HasForeignKey(d => d.UserDataId); // Keep existing FK name for compatibility

                        
                builder.Entity<AppUser>()
                        .HasMany(u => u.Applications)
                        .WithOne(a => a.User)
                        .HasForeignKey(a => a.UserId)
                        .IsRequired();

                // Configure UniProgram -> Applications relationship
                builder.Entity<UniProgram>()
                    .HasMany(p => p.Applications)
                    .WithOne(a => a.Program)
                    .HasForeignKey(a => a.ProgramId)
                    .IsRequired();





        }
}
