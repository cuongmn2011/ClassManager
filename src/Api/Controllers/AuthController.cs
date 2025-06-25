// File: src/Api/Controllers/AuthController.cs
using Application.DTOs.Auth;
using Application.DTOs.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (!result.IsSuccess)
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid username or password.", 401));
            }

            var responseDto = new LoginResponseDto
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken,
                User = result.User
            };

            return Ok(ApiResponse<LoginResponseDto>.Success(responseDto));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var result = await _authService.RefreshTokenAsync(requestDto.RefreshToken);

            if (!result.IsSuccess)
            {
                return Unauthorized(ApiResponse<object>.Fail(result.ErrorMessage ?? "Invalid request.", 401));
            }

            var responseDto = new LoginResponseDto
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken,
                User = result.User
            };

            return Ok(ApiResponse<LoginResponseDto>.Success(responseDto));
        }
    }
}