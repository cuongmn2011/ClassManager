// File: src/Infrastructure/Services/UserService.cs
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IFileStorageService _fileStorageService;
        public UserService(UserManager<User> userManager, IFileStorageService fileStorageService)
        {
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }
            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }

        public async Task<(bool Succeeded, UserDto? User, IEnumerable<string>? Errors)> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                FullName = createUserDto.FullName,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                return (false, null, result.Errors.Select(e => e.Description));
            }
            await _userManager.AddToRolesAsync(user, createUserDto.Roles);
            var userDto = await GetUserByIdAsync(user.Id);
            return (true, userDto, null);
        }

        public async Task<(bool Succeeded, IEnumerable<string>? Errors)> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, new[] { "User not found." });

            user.FullName = updateUserDto.FullName;
            user.Email = updateUserDto.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return (false, updateResult.Errors.Select(e => e.Description));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(updateUserDto.Roles);
            var rolesToAdd = updateUserDto.Roles.Except(currentRoles);

            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            return (true, null);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<(bool Succeeded, string? newAvatarUrl)> UpdateUserAvatarAsync(string userId, IFormFile avatarFile)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, null);
            }

            // Delete the old avatar if it exists
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                _fileStorageService.DeleteFile(user.AvatarUrl);
            }

            // Save the new avatar and get the public URL
            var newAvatarUrl = await _fileStorageService.SaveFileAsync(avatarFile, "uploads/avatars");

            // Update the user's record in the database
            user.AvatarUrl = newAvatarUrl;
            await _userManager.UpdateAsync(user);

            return (true, newAvatarUrl);
        }
    }
}