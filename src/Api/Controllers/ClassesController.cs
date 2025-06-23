// File: src/Api/Controllers/ClassesController.cs
using Application.DTOs.Classes;
using Application.DTOs.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;
        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Classes.View)]
        public async Task<IActionResult> GetClasses()
        {
            var classes = await _classService.GetClassesAsync();
            return Ok(ApiResponse<IEnumerable<ClassDto>>.Success(classes.ToList()));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.Classes.View)]
        public async Task<IActionResult> GetClassById(string id)
        {
            var @class = await _classService.GetClassByIdAsync(id);
            if (@class == null) return NotFound(ApiResponse<ClassDto>.Fail("Class not found.", 404));
            return Ok(ApiResponse<ClassDto>.Success(@class));
        }

        [HttpPost]
        [Authorize(Policy = Permissions.Classes.Create)]
        public async Task<IActionResult> CreateClass([FromBody] CreateUpdateClassDto createDto)
        {
            var newClass = await _classService.CreateClassAsync(createDto);
            var response = ApiResponse<ClassDto>.Success(newClass, 201);
            return CreatedAtAction(nameof(GetClassById), new { id = newClass.ClassId }, response);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.Classes.Edit)]
        public async Task<IActionResult> UpdateClass(string id, [FromBody] CreateUpdateClassDto updateDto)
        {
            var success = await _classService.UpdateClassAsync(id, updateDto);
            if (!success) return NotFound(ApiResponse<bool>.Fail("Class not found to update.", 404));
            return Ok(ApiResponse<string>.Success(id));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.Classes.Delete)]
        public async Task<IActionResult> DeleteClass(string id)
        {
            var success = await _classService.DeleteClassAsync(id);
            if (!success) return NotFound(ApiResponse<bool>.Fail("Class not found to delete.", 404));
            return Ok(ApiResponse<object>.Success(null));
        }
    }
}