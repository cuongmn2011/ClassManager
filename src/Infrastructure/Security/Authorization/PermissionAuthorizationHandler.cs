// File: src/Infrastructure/Security/Authorization/PermissionAuthorizationHandler.cs
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Infrastructure.Security.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public PermissionAuthorizationHandler(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Get the user ID from the claims
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return; // User is not authenticated
            }

            // Get the user's roles
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            // Check if any of the user's roles has the required permission
            var hasPermission = await _dbContext.RolePermissions
                .Where(rp => userRoles.Contains(rp.Role.Name))
                .Include(rp => rp.Permission)
                .AnyAsync(rp => rp.Permission.Name == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}