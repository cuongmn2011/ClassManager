// File: src/Application/Interfaces/IUserService.cs
using Application.DTOs.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<(bool Succeeded, UserDto? User, IEnumerable<string>? Errors)> CreateUserAsync(CreateUserDto createUserDto);
        Task<(bool Succeeded, IEnumerable<string>? Errors)> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<(bool Succeeded, string? newAvatarUrl)> UpdateUserAvatarAsync(string userId, IFormFile avatarFile);
    }
}