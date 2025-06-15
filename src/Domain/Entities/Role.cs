using Microsoft.AspNetCore.Identity;
namespace Domain.Entities
{
    public class Role : IdentityRole
    {
        // Example of a custom property we        dotnet build could add later:
        // public string? Description { get; set; }
        public string? Description { get; set; }
    }
}