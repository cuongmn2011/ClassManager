using System;
namespace Application.DTOs.Teachers
{
    public class CreateTeacherDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty; // Teacher-specific field
    }
}