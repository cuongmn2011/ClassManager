using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    // Inherit from IdentityDbContext instead of the regular DbContext.
    // This automatically adds all the necessary tables for ASP.NET Core Identity.
    // We specify our custom User and Role classes, with string as the primary key type.
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Class> Classes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // We don't need to declare "public DbSet<User> Users { get; set; }"
        // or "public DbSet<Role> Roles { get; set; }"
        // because IdentityDbContext already handles them for us.

        // We will add DbSets for other entities (like Student, Teacher, Class) here later.

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configure entity relationships and properties here if needed in the future.
            // Change name table of ASP.NET Core Identity
            builder.Entity<User>(entity => { entity.ToTable(name: "Users"); });
            builder.Entity<Role>(entity => { entity.ToTable(name: "Roles"); });
            builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
            builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });

            // Configure the many-to-many relationship between Role and Permission
            // using the RolePermission join table.
            builder.Entity<RolePermission>(entity =>
            {
                // Set the composite primary key
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                // Configure the relationship to the Role entity
                entity.HasOne(rp => rp.Role)
                    .WithMany() // A Role can have many RolePermissions
                    .HasForeignKey(rp => rp.RoleId);

                // Configure the relationship to the Permission entity
                entity.HasOne(rp => rp.Permission)
                    .WithMany() // A Permission can be in many RolePermissions
                    .HasForeignKey(rp => rp.PermissionId);

                // Set the table name for the join table
                entity.ToTable("RolePermissions");
            });
            
            // Configure the Class entity
            builder.Entity<Class>(entity =>
            {
                // Specify the column type for FeeAmount to avoid data truncation
                entity.Property(c => c.FeeAmount).HasColumnType("decimal(18, 2)");
            });
        }
    }
}