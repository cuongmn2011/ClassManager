// File: src/Domain/Entities/Class.cs
namespace Domain.Entities
{
    public class Class
    {
        public int Id { get; set; } // Primary Key

        // Foreign Key to Branch table
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public string ClassName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}