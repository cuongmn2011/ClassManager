// File: src/Api/Controllers/StudentsController.cs
using Application.DTOs.Common; // Add this using
using Application.DTOs.Students;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetStudents()
        {
            var students = await _studentService.GetStudentsAsync();
            // Wrap the result in our standard ApiResponse
            var response = ApiResponse<IEnumerable<StudentDto>>.Success(students.ToList());
            return Ok(response);
        }

        [HttpGet("{id}")]
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
            return Ok(ApiResponse<bool>.Success(true, 200)); // Or return NoContent() with a custom response if needed.
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var success = await _studentService.DeleteStudentAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.Fail("Student not found to delete.", 404));
            }
            return Ok(ApiResponse<bool>.Success(true, 200));
        }
    }
}