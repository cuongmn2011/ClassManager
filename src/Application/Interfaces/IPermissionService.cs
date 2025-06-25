// File: src/Application/Interfaces/IPermissionService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Roles;

namespace Application.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermissionsForUserAsync(string userId);
        Task<bool> UpdatePermissionsForRoleAsync(string roleName, List<string> newPermissions);
        Task<List<string>> GetAssignablePermissionsAsync();
        Task<List<RoleDto>> GetRolesAsync();
        Task<RoleWithPermissionsDto?> GetRoleWithPermissionsAsync(string roleName);
    }
}