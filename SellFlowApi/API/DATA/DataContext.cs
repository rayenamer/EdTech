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
        public DbSet<UniProgram> UniPrograms { get; set; }
        public DbSet<Application> Applications { get; set; }
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
                builder.Entity<Application>()
                        .HasOne(a => a.User)
                        .WithMany(u => u.Applications)
                        .HasForeignKey(a => a.UserId);

                builder.Entity<Application>()
                        .HasOne(a => a.Program)
                        .WithMany(p => p.Applications)
                        .HasForeignKey(a => a.ProgramId);
                //doc
                builder.Entity<Application>()
                        .HasMany(a => a.Documents)
                        .WithOne(d => d.Application)
                        .HasForeignKey(d => d.ApplicationId)
                        .OnDelete(DeleteBehavior.Cascade);

        }
}
