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
        public DbSet<UserData> UserDatas { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
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

                // AppUser ↔ UserData (1:1)
                builder.Entity<AppUser>()
                    .HasOne(u => u.UserData)
                    .WithOne(ud => ud.User)
                    .HasForeignKey<UserData>(ud => ud.UserId); // FK in UserData

                // UserData ↔ Documents (1:many)
                builder.Entity<UserData>()
                    .HasMany(ud => ud.Documents)
                    .WithOne(d => d.UserData)
                    .HasForeignKey(d => d.UserDataId)
                    .OnDelete(DeleteBehavior.Cascade);


        }
}
