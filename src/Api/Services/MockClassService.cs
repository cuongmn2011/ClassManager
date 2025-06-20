// File: src/Api/Services/MockClassService.cs
using Application.DTOs.Classes;
using Application.Interfaces;

namespace Api.Services
{
    public class MockClassService : IClassService
    {
        private static readonly List<ClassDto> _classes = new()
        {
            new ClassDto { ClassId = "CLS01", BranchId = "BRA01", ClassName = "Lớp Toán Nâng Cao A1", Description = "Lớp dành cho học sinh giỏi toán", FeeAmount = 1500000 },
            new ClassDto { ClassId = "CLS02", BranchId = "BRA01", ClassName = "Lớp Lý Cơ Bản B2", Description = "Lớp cho người mới bắt đầu", FeeAmount = 1200000 },
            new ClassDto { ClassId = "CLS03", BranchId = "BRA02", ClassName = "Lớp Anh Văn Giao Tiếp C1", Description = "Tập trung vào kỹ năng nói", FeeAmount = 2000000 }
        };

        public Task<ClassDto> CreateClassAsync(CreateUpdateClassDto createDto)
        {
            var newClass = new ClassDto
            {
                ClassId = $"CLS{new Random().Next(10, 99)}",
                BranchId = createDto.BranchId,
                ClassName = createDto.ClassName,
                Description = createDto.Description,
                FeeAmount = createDto.FeeAmount
            };
            _classes.Add(newClass);
            return Task.FromResult(newClass);
        }

        public Task<bool> DeleteClassAsync(string classId)
        {
            var @class = _classes.FirstOrDefault(c => c.ClassId == classId);
            if (@class == null) return Task.FromResult(false);
            _classes.Remove(@class);
            return Task.FromResult(true);
        }

        public Task<ClassDto?> GetClassByIdAsync(string classId)
        {
            return Task.FromResult(_classes.FirstOrDefault(c => c.ClassId == classId));
        }

        public Task<IEnumerable<ClassDto>> GetClassesAsync()
        {
            return Task.FromResult<IEnumerable<ClassDto>>(_classes);
        }

        public Task<bool> UpdateClassAsync(string classId, CreateUpdateClassDto updateDto)
        {
            var @class = _classes.FirstOrDefault(c => c.ClassId == classId);
            if (@class == null) return Task.FromResult(false);
            @class.BranchId = updateDto.BranchId;
            @class.ClassName = updateDto.ClassName;
            @class.Description = updateDto.Description;
            @class.FeeAmount = updateDto.FeeAmount;
            return Task.FromResult(true);
        }
    }
}