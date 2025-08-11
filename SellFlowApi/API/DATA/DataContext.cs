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
          AppUser ,
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
        }
}
