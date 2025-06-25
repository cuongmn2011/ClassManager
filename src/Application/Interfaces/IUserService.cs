// File: src/Application/Interfaces/IUserService.cs
using Application.DTOs.Users;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<(bool Succeeded, UserDto? User, IEnumerable<string>? Errors)> CreateUserAsync(CreateUserDto createUserDto);
        Task<(bool Succeeded, IEnumerable<string>? Errors)> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}