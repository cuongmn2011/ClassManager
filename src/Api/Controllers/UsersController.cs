// File: src/Api/Controllers/UsersController.cs
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Users.View)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.Success(users.ToList()));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.Users.View)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(ApiResponse<UserDto>.Fail("User not found.", 404));
            return Ok(ApiResponse<UserDto>.Success(user));
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Users.Create)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var result = await _userService.CreateUserAsync(createUserDto);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<UserDto>.Fail(result.Errors.ToList()));
            }
            var response = ApiResponse<UserDto>.Success(result.User, 201);
            return CreatedAtAction(nameof(GetUserById), new { id = result.User.Id }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.Users.Edit)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateUserAsync(id, updateUserDto);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Errors.ToList()));
            }
            return Ok(ApiResponse<string>.Success(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.Users.Delete)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(ApiResponse<bool>.Fail("User not found.", 404));
            return Ok(ApiResponse<object>.Success(null));
        }

        [HttpPost("{id}/avatar")]
        [Authorize(Policy = Permissions.Users.Edit)]
        public async Task<IActionResult> UploadAvatar(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.Fail("No file uploaded."));
            }

            var result = await _userService.UpdateUserAvatarAsync(id, file);

            if (!result.Succeeded)
            {
                return NotFound(ApiResponse<object>.Fail("User not found."));
            }

            return Ok(ApiResponse<object>.Success(new { avatarUrl = result.newAvatarUrl }));
        }
    }
}