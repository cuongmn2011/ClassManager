// File: src/Domain/Entities/UserRefreshToken.cs
using System;

namespace Domain.Entities
{
    public class UserRefreshToken
    {
        public int Id { get; set; } // Primary key
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public string Token { get; set; } = string.Empty; // The actual refresh token string
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}