// File: src/Infrastructure/Services/PermissionService.cs
using Application.Interfaces;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Constants;
using Application.DTOs.Roles;

namespace Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public PermissionService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<string>> GetPermissionsForUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            var userRoles = await _userManager.GetRolesAsync(user);

            var permissions = await _context.RolePermissions
                .Where(rp => userRoles.Contains(rp.Role.Name))
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            return permissions;
        }

        public async Task<bool> UpdatePermissionsForRoleAsync(string roleName, List<string> newPermissionNames)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null) return false;

            // Get all valid permissions from the database that match the new list
            var allPermissions = await _context.Permissions.ToListAsync();
            var newPermissions = allPermissions.Where(p => newPermissionNames.Contains(p.Name)).ToList();

            // Remove old permissions for this role
            var oldRolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == role.Id);
            _context.RolePermissions.RemoveRange(oldRolePermissions);

            // Add new permissions for this role
            foreach (var permission in newPermissions)
            {
                _context.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<string>> GetAssignablePermissionsAsync()
        {
            // Get all permissions defined in the static Permissions class
            var allPermissions = Permissions.GetAllPermissions();

            // Define a list of permissions to exclude (system/admin level)
            var excludedPermissions = new List<string>
            {
                Permissions.Roles.View,
                Permissions.Roles.Edit
            };

            // Filter out the excluded permissions
            var assignablePermissions = allPermissions.Except(excludedPermissions).ToList();

            return Task.FromResult(assignablePermissions);
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<RoleWithPermissionsDto?> GetRoleWithPermissionsAsync(string roleName)
        {
            var role = await _context.Roles
                .Where(r => r.Name == roleName)
                .Select(r => new RoleWithPermissionsDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Permissions = _context.RolePermissions
                                    .Where(rp => rp.RoleId == r.Id)
                                    .Select(rp => rp.Permission.Name)
                                    .ToList()
                })
                .FirstOrDefaultAsync();

            return role;
        }
    }
}