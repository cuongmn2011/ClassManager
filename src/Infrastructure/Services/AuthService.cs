// File: src/Infrastructure/Services/AuthService.cs
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly IPermissionService _permissionService;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, ApplicationDbContext context, IPermissionService permissionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            _permissionService = permissionService;
        }

        public async Task<(bool IsSuccess, string? Token, string? RefreshToken, UserInfo? User)> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null || !await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false).ContinueWith(t => t.Result.Succeeded))
            {
                return (false, null, null, null);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _permissionService.GetPermissionsForUserAsync(user.Id);
            var token = _tokenService.CreateToken(user, roles);
            var refreshToken = await _tokenService.CreateAndSaveRefreshTokenAsync(user);

            var userInfo = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                Permissions = permissions
            };

            return (true, token, refreshToken, userInfo);
        }

        public async Task<(bool IsSuccess, string? Token, string? RefreshToken, UserInfo? User, string? ErrorMessage)> RefreshTokenAsync(string refreshToken)
        {
            var savedRefreshToken = await _context.UserRefreshTokens
                .Include(urt => urt.User)
                .FirstOrDefaultAsync(urt => urt.Token == refreshToken);

            if (savedRefreshToken == null || savedRefreshToken.IsRevoked || savedRefreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return (false, null, null, null, "Invalid or expired refresh token.");
            }

            savedRefreshToken.IsRevoked = true;

            var user = savedRefreshToken.User;
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.CreateToken(user, roles);
            var newRefreshToken = await _tokenService.CreateAndSaveRefreshTokenAsync(user);

            await _context.SaveChangesAsync();

            var userInfo = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl
            };

            return (true, newAccessToken, newRefreshToken, userInfo, null);
        }
    }
}