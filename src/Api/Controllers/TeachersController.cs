// File: src/Api/Controllers/TeachersController.cs
using Application.DTOs.Common;
using Application.DTOs.Teachers;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Teachers.View)]
        public async Task<IActionResult> GetTeachers()
        {
            var teachers = await _teacherService.GetTeachersAsync();
            return Ok(ApiResponse<IEnumerable<TeacherDto>>.Success(teachers.ToList()));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.Teachers.View)]
        public async Task<IActionResult> GetTeacherById(string id)
        {
            var teacher = await _teacherService.GetTeacherByIdAsync(id);
            if (teacher == null)
            {
                return NotFound(ApiResponse<TeacherDto>.Fail("Teacher not found.", 404));
            }
            return Ok(ApiResponse<TeacherDto>.Success(teacher));
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Teachers.Create)]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto createTeacherDto)
        {
            var newTeacher = await _teacherService.CreateTeacherAsync(createTeacherDto);
            var response = ApiResponse<TeacherDto>.Success(newTeacher, 201);
            return CreatedAtAction(nameof(GetTeacherById), new { id = newTeacher.TeacherId }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.Teachers.Edit)]
        public async Task<IActionResult> UpdateTeacher(string id, [FromBody] UpdateTeacherDto updateTeacherDto)
        {
            var success = await _teacherService.UpdateTeacherAsync(id, updateTeacherDto);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.Fail("Teacher not found to update.", 404));
            }
            return Ok(ApiResponse<string>.Success(id, 200));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.Teachers.Delete)]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var success = await _teacherService.DeleteTeacherAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<bool>.Fail("Teacher not found to delete.", 404));
            }
            return Ok(ApiResponse<object>.Success(null, 200));
        }
    }
}