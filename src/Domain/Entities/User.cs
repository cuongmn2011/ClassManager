using Microsoft.AspNetCore.Identity;
namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; } // Nullable for newly created users who haven't logged in.
        public bool IsDeleted { get; set; } = false;
    }
}