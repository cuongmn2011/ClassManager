// File: src/Application/DTOs/Auth/RefreshTokenRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}