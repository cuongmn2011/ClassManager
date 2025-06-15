// File: src/Application/DTOs/Teachers/TeacherDto.cs
using System;
namespace Application.DTOs.Teachers
{
    public class TeacherDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string AvatarUrl { get; set; }= string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Field specific to Teacher entity
        public string Specialization { get; set; } = string.Empty;
    }
}