// File: src/Api/Controllers/StudentsController.cs
using Application.DTOs.Common; // Add this using
using Application.DTOs.Students;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Students.View)]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentService.GetStudentsAsync();
            // Wrap the result in our standard ApiResponse
            var response = ApiResponse<IEnumerable<StudentDto>>.Success(students.ToList());
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.Students.View)]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                // Return the standard ApiResponse for a failure case
                var response = ApiResponse<StudentDto>.Fail("Student not found.", 404);
                return NotFound(response);
            }
            // Wrap the successful result
            return Ok(ApiResponse<StudentDto>.Success(student));
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Students.Create)]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto createStudentDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<StudentDto>.Fail(errors));
            }
            var newStudent = await _studentService.CreateStudentAsync(createStudentDto);
            var response = ApiResponse<StudentDto>.Success(newStudent, 201);
            return CreatedAtAction(nameof(GetStudentById), new { id = newStudent.StudentId }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.Students.Edit)]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDto updateStudentDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<StudentDto>.Fail(errors));
            }
            var success = await _studentService.UpdateStudentAsync(id, updateStudentDto);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.Fail("Student not found to update.", 404));
            }
            return Ok(ApiResponse<string>.Success(id, 200)); // Or return NoContent() with a custom response if needed.
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.Students.Delete)]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var success = await _studentService.DeleteStudentAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.Fail("Student not found to delete.", 404));
            }
            return Ok(ApiResponse<object>.Success(null, 200));
        }
        [HttpPost("{id}/avatar")]
        [Authorize(Policy = Permissions.Students.Edit)] 
        public async Task<IActionResult> UploadAvatar(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.Fail("No file uploaded."));
            }

            var result = await _studentService.UpdateStudentAvatarAsync(id, file);

            if (!result.Succeeded)
            {
                return NotFound(ApiResponse<object>.Fail("Student not found or failed to update avatar."));
            }

            return Ok(ApiResponse<object>.Success(new { avatarUrl = result.newAvatarUrl }));
        }
    }
}