using System;
namespace Application.DTOs.Students
{
    public class CreateStudentDto
    {
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
    }
}