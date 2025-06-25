// File: src/Application/Interfaces/IBranchService.cs
using Application.DTOs.Branches;

namespace Application.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetBranchesAsync();
        Task<BranchDto?> GetBranchByIdAsync(string branchId);
        Task<BranchDto> CreateBranchAsync(CreateUpdateBranchDto createDto);
        Task<bool> UpdateBranchAsync(string branchId, CreateUpdateBranchDto updateDto);
        Task<bool> DeleteBranchAsync(string branchId);
    }
}