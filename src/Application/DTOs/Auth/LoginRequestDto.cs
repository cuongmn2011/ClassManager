// File: src/Application/DTOs/Auth/LoginRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        // Changed from Email to UserName
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}