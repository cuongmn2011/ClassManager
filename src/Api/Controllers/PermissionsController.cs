// File: src/Api/Controllers/PermissionsController.cs
using Application.DTOs.Common;
using Application.DTOs.Permissions;
using Application.Interfaces;
using Domain.Constants; // Add this using
using Microsoft.AspNetCore.Authorization; // Add this using
using Microsoft.AspNetCore.Mvc;
using System; // Add this using
using System.Threading.Tasks;
using Application.DTOs.Roles;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    // Only authenticated users can access this controller
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // The API to update permissions for a specific role
        [HttpPut("roles/{roleName}")]
        // Secure this endpoint: Only users with the 'Permissions.Roles.Edit' permission can access it.
        [Authorize(Policy = Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateRolePermissions(string roleName, [FromBody] UpdateRolePermissionsRequestDto request)
        {
            // 1. Prevent editing the 'Admin' role's permissions
            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail("Cannot edit Admin role permissions.", 400));
            }

            // 2. Only allow editing 'Teacher' and 'Student' roles
            if (!roleName.Equals("Teacher", StringComparison.OrdinalIgnoreCase) &&
                !roleName.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail("Can only edit permissions for 'Teacher' or 'Student' roles.", 400));
            }

            var success = await _permissionService.UpdatePermissionsForRoleAsync(roleName, request.Permissions);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.Fail("Role not found.", 404));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        /// <summary>
        /// Gets a list of all assignable permissions for UI display.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = Permissions.Roles.View)] // Only users who can view roles can see the permissions list
        public async Task<IActionResult> GetAllAssignablePermissions()
        {
            var permissions = await _permissionService.GetAssignablePermissionsAsync();
            return Ok(ApiResponse<List<string>>.Success(permissions));
        }

        [HttpGet("roles")]
        [Authorize(Policy = Permissions.Roles.View)]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _permissionService.GetRolesAsync();
            return Ok(ApiResponse<List<RoleDto>>.Success(roles));
        }
        
        [HttpGet("roles/{roleName}")]
        [Authorize(Policy = Permissions.Roles.View)]
        public async Task<IActionResult> GetRoleDetails(string roleName)
        {
            var role = await _permissionService.GetRoleWithPermissionsAsync(roleName);
            if (role == null)
            {
                return NotFound(ApiResponse<object>.Fail("Role not found.", 404));
            }
            return Ok(ApiResponse<RoleWithPermissionsDto>.Success(role));
        }
    }
}