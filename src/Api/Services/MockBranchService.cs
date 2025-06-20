// File: src/Api/Services/MockBranchService.cs
using Application.DTOs.Branches;
using Application.Interfaces;

namespace Api.Services
{
    public class MockBranchService : IBranchService
    {
        private static readonly List<BranchDto> _branches = new()
        {
            new BranchDto { BranchId = "BRA01", BranchName = "Chi nhánh Cầu Giấy", BranchAddress = "123 Xuân Thủy, Cầu Giấy, Hà Nội" },
            new BranchDto { BranchId = "BRA02", BranchName = "Chi nhánh Quận 1", BranchAddress = "456 Lê Lợi, Quận 1, TP. HCM" }
        };

        public Task<BranchDto> CreateBranchAsync(CreateUpdateBranchDto createDto)
        {
            var newBranch = new BranchDto
            {
                BranchId = $"BRA{new Random().Next(10, 99)}",
                BranchName = createDto.BranchName,
                BranchAddress = createDto.BranchAddress
            };
            _branches.Add(newBranch);
            return Task.FromResult(newBranch);
        }

        public Task<bool> DeleteBranchAsync(string branchId)
        {
            var branch = _branches.FirstOrDefault(b => b.BranchId == branchId);
            if (branch == null) return Task.FromResult(false);
            _branches.Remove(branch);
            return Task.FromResult(true);
        }

        public Task<BranchDto?> GetBranchByIdAsync(string branchId)
        {
            return Task.FromResult(_branches.FirstOrDefault(b => b.BranchId == branchId));
        }

        public Task<IEnumerable<BranchDto>> GetBranchesAsync()
        {
            return Task.FromResult<IEnumerable<BranchDto>>(_branches);
        }

        public Task<bool> UpdateBranchAsync(string branchId, CreateUpdateBranchDto updateDto)
        {
            var branch = _branches.FirstOrDefault(b => b.BranchId == branchId);
            if (branch == null) return Task.FromResult(false);
            branch.BranchName = updateDto.BranchName;
            branch.BranchAddress = updateDto.BranchAddress;
            return Task.FromResult(true);
        }
    }
}