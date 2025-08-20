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

                // AppUser ↔ UserData (1:1)
                builder.Entity<AppUser>()
                        .HasOne(u => u.UserData)
                        .WithOne(ud => ud.User)
                        .HasForeignKey<UserData>(ud => ud.UserId); // FK in UserData
                builder.Entity<UserData>()
                        .HasMany(ud => ud.Documents)
                        .WithOne()  // Empty WithOne() since Document has no navigation property back to UserData
                        .HasForeignKey(d => d.UserDataId);

                        
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

                /*builder.Entity<UserData>()
                    .HasMany(ud => ud.Documents)
                    .WithOne(d => d.UserData)
                    .HasForeignKey(d => d.UserDataId); // FK in Document*/


                /*HasMany(ud => ud.Documents) - UserData has many Documents
                WithOne(d => d.UserData) - Each Document has one UserData (navigation property)
                HasForeignKey(d => d.UserDataId) - The foreign key in Document table*/





        }
}
