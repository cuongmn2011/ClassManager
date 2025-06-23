// File: src/Domain/Entities/Permission.cs
namespace Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; } = string.Empty; // e.g., "Permissions.Students.View"
    }
}