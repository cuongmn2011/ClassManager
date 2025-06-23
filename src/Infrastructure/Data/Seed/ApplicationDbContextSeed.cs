// File: src/Infrastructure/Data/Seed/ApplicationDbContextSeed.cs
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Seed
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAndRolesAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            // Seed Roles
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new Role { Name = "Admin", Description = "Administrator role with full rights" });
                await roleManager.CreateAsync(new Role { Name = "Teacher", Description = "Teacher role with teaching rights" });
                await roleManager.CreateAsync(new Role { Name = "Student", Description = "Student role with limited rights" });
            }

            // Seed Default Admin User
            if (!userManager.Users.Any())
            {
                var defaultAdmin = new User
                {
                    UserName = "admin",
                    Email = "cuongmn@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(defaultAdmin, "m@nhCuong201196");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                }
            }
        }
    }
}