// File: src/Api/Controllers/AuthController.cs
using Application.DTOs.Auth;
using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Handles user login.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>A JWT on successful login.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // Find the user by their user name.
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            // Check if user exists and password is correct.
            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid username or password.", 401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid username or password.", 401));
            }

            // If login is successful, get user roles and create a token.
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            var response = new LoginResponseDto { Token = token };

            return Ok(ApiResponse<LoginResponseDto>.Success(response));
        }
    }
}