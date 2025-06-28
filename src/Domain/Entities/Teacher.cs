// File: src/Domain/Entities/Teacher.cs
namespace Domain.Entities
{
    public class Teacher
    {
        public int Id { get; set; } // Primary Key

        // Liên kết với bảng Users
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        // Các trường thông tin giống DTO
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        // Các trường ngày tháng
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}