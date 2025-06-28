// File: src/Domain/Entities/Branch.cs
namespace Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; } // Primary Key
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}