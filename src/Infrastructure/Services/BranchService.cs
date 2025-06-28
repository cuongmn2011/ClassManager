// File: src/Infrastructure/Services/BranchService.cs
using Application.DTOs.Branches;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;

        public BranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchDto>> GetBranchesAsync()
        {
            return await _context.Branches
                .Where(b => !b.IsDeleted)
                .Select(b => new BranchDto
                {
                    BranchId = b.Id.ToString(),
                    BranchName = b.BranchName,
                    BranchAddress = b.BranchAddress
                }).ToListAsync();
        }

        public async Task<BranchDto?> GetBranchByIdAsync(string branchId)
        {
            if (!int.TryParse(branchId, out int id)) return null;

            return await _context.Branches
                .Where(b => b.Id == id && !b.IsDeleted)
                .Select(b => new BranchDto
                {
                    BranchId = b.Id.ToString(),
                    BranchName = b.BranchName,
                    BranchAddress = b.BranchAddress
                }).FirstOrDefaultAsync();
        }

        public async Task<BranchDto> CreateBranchAsync(CreateUpdateBranchDto createDto)
        {
            var branch = new Branch
            {
                BranchName = createDto.BranchName,
                BranchAddress = createDto.BranchAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Branches.AddAsync(branch);
            await _context.SaveChangesAsync();

            return (await GetBranchByIdAsync(branch.Id.ToString()))!;
        }

        public async Task<bool> UpdateBranchAsync(string branchId, CreateUpdateBranchDto updateDto)
        {
            if (!int.TryParse(branchId, out int id)) return false;

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null || branch.IsDeleted) return false;

            branch.BranchName = updateDto.BranchName;
            branch.BranchAddress = updateDto.BranchAddress;
            branch.UpdatedAt = DateTime.UtcNow;

            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBranchAsync(string branchId)
        {
            if (!int.TryParse(branchId, out int id)) return false;

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return false;

            branch.IsDeleted = true; // Soft delete
            branch.UpdatedAt = DateTime.UtcNow;

            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}