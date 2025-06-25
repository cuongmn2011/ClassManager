// File: src/Application/DTOs/Permissions/UpdateRolePermissionsRequestDto.cs
using System.Collections.Generic;

namespace Application.DTOs.Permissions
{
    public class UpdateRolePermissionsRequestDto
    {
        public List<string> Permissions { get; set; } = new();
    }
}