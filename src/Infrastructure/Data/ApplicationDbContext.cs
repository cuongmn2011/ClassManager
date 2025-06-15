using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    // Inherit from IdentityDbContext instead of the regular DbContext.
    // This automatically adds all the necessary tables for ASP.NET Core Identity.
    // We specify our custom User and Role classes, with string as the primary key type.
    public class ApplicationDbContext : IdentityDbContext<User, Role, string>
    {
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
        }
    }
}