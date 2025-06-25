// File: src/Application/DTOs/Roles/RoleDto.cs
namespace Application.DTOs.Roles
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}