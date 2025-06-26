// File: src/Infrastructure/Data/Seed/ApplicationDbContextSeed.cs
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                    FullName = "Administrator",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(defaultAdmin, "m@nhCuong201196");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                }

                //Create the second admin user
                var defaultAdmin2 = new User
                {
                    UserName = "admin2",
                    Email = "cuongmn2@gmail.com",
                    FullName = "Administrator 2",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result2 = await userManager.CreateAsync(defaultAdmin2, "m@nhCuong201196"); // Using the same password for convenience

                if (result2.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin2, "Admin");
                }
            }
        }

        public static async Task SeedPermissionsAsync(ApplicationDbContext context, RoleManager<Role> roleManager)
        {
            // 1. Seed Permissions from the static class to the database
            var allPermissions = Permissions.GetAllPermissions();
            var existingPermissions = await context.Permissions.Select(p => p.Name).ToListAsync();

            var newPermissions = allPermissions.Except(existingPermissions);

            foreach (var permissionName in newPermissions)
            {
                await context.Permissions.AddAsync(new Permission { Name = permissionName });
            }
            await context.SaveChangesAsync();

            // 2. Grant all permissions to the Admin role
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var allDbPermissions = await context.Permissions.ToListAsync();
                var currentAdminPermissions = await context.RolePermissions
                                                    .Where(rp => rp.RoleId == adminRole.Id)
                                                    .Select(rp => rp.PermissionId)
                                                    .ToListAsync();

                foreach (var permission in allDbPermissions)
                {
                    if (!currentAdminPermissions.Contains(permission.Id))
                    {
                        await context.RolePermissions.AddAsync(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id
                        });
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}