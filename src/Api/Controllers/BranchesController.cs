// File: src/Api/Controllers/BranchesController.cs
using Application.DTOs.Branches;
using Application.DTOs.Common;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _branchService.GetBranchesAsync();
            return Ok(ApiResponse<IEnumerable<BranchDto>>.Success(branches.ToList()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(string id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null) return NotFound(ApiResponse<BranchDto>.Fail("Branch not found.", 404));
            return Ok(ApiResponse<BranchDto>.Success(branch));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] CreateUpdateBranchDto createDto)
        {
            var newBranch = await _branchService.CreateBranchAsync(createDto);
            var response = ApiResponse<BranchDto>.Success(newBranch, 201);
            return CreatedAtAction(nameof(GetBranchById), new { id = newBranch.BranchId }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(string id, [FromBody] CreateUpdateBranchDto updateDto)
        {
            var success = await _branchService.UpdateBranchAsync(id, updateDto);
            if (!success) return NotFound(ApiResponse<bool>.Fail("Branch not found to update.", 404));
            // Using the response style you prefer
            return Ok(ApiResponse<string>.Success(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(string id)
        {
            var success = await _branchService.DeleteBranchAsync(id);
            if (!success) return NotFound(ApiResponse<bool>.Fail("Branch not found to delete.", 404));
            // Using the response style you prefer
            return Ok(ApiResponse<object>.Success(null));
        }
    }
}