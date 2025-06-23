// File: src/Domain/Entities/RolePermission.cs
namespace Domain.Entities
{
    public class RolePermission
    {
        // Foreign key for the Role
        public string RoleId { get; set; } = string.Empty;
        public Role Role { get; set; } = null!;

        // Foreign key for the Permission
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}