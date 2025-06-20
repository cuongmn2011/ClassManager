// File: src/Application/Interfaces/IClassService.cs
using Application.DTOs.Classes;

namespace Application.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassDto>> GetClassesAsync();
        Task<ClassDto?> GetClassByIdAsync(string classId);
        Task<ClassDto> CreateClassAsync(CreateUpdateClassDto createDto);
        Task<bool> UpdateClassAsync(string classId, CreateUpdateClassDto updateDto);
        Task<bool> DeleteClassAsync(string classId);
    }
}