// File: src/Infrastructure/Security/Authorization/PermissionRequirement.cs
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Security.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}