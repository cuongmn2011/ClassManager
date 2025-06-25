// File: src/Application/Interfaces/IAuthService.cs
using Application.DTOs.Auth;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string? Token, string? RefreshToken, UserInfo? User)> LoginAsync(LoginRequestDto loginDto);
        Task<(bool IsSuccess, string? Token, string? RefreshToken, UserInfo? User, string? ErrorMessage)> RefreshTokenAsync(string refreshToken);
    }
}