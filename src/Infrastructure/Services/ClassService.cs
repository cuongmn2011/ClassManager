// File: src/Infrastructure/Services/ClassService.cs
using Application.DTOs.Classes;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;
        public ClassService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassDto>> GetClassesAsync()
        {
            return await _context.Classes
                .Where(c => !c.IsDeleted)
                .Select(c => new ClassDto
                {
                    ClassId = c.Id.ToString(),
                    BranchId = c.BranchId.ToString(),
                    ClassName = c.ClassName,
                    Description = c.Description,
                    FeeAmount = c.FeeAmount
                }).ToListAsync();
        }
        
        public async Task<ClassDto?> GetClassByIdAsync(string classId)
        {
            if (!int.TryParse(classId, out int id)) return null;

            return await _context.Classes
                .Where(c => c.Id == id && !c.IsDeleted)
                .Select(c => new ClassDto
                {
                    ClassId = c.Id.ToString(),
                    BranchId = c.BranchId.ToString(),
                    ClassName = c.ClassName,
                    Description = c.Description,
                    FeeAmount = c.FeeAmount
                }).FirstOrDefaultAsync();
        }

        public async Task<ClassDto> CreateClassAsync(CreateUpdateClassDto createDto)
        {
            var @class = new Class
            {
                BranchId = int.Parse(createDto.BranchId), // Assuming BranchId is passed as string
                ClassName = createDto.ClassName,
                Description = createDto.Description,
                FeeAmount = createDto.FeeAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Classes.AddAsync(@class);
            await _context.SaveChangesAsync();

            return (await GetClassByIdAsync(@class.Id.ToString()))!;
        }

        public async Task<bool> UpdateClassAsync(string classId, CreateUpdateClassDto updateDto)
        {
            if (!int.TryParse(classId, out int id)) return false;

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null || @class.IsDeleted) return false;

            @class.BranchId = int.Parse(updateDto.BranchId);
            @class.ClassName = updateDto.ClassName;
            @class.Description = updateDto.Description;
            @class.FeeAmount = updateDto.FeeAmount;
            @class.UpdatedAt = DateTime.UtcNow;

            _context.Classes.Update(@class);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(string classId)
        {
            if (!int.TryParse(classId, out int id)) return false;

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return false;

            @class.IsDeleted = true; // Soft delete
            @class.UpdatedAt = DateTime.UtcNow;

            _context.Classes.Update(@class);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}