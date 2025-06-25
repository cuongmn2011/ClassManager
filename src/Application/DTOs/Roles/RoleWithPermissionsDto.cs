// File: src/Application/DTOs/Roles/RoleWithPermissionsDto.cs
using System.Collections.Generic;

namespace Application.DTOs.Roles
{
    public class RoleWithPermissionsDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}