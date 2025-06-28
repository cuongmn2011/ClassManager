using Application.DTOs.Teachers;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ITeacherService
    {
        /// <summary>
        /// Gets a list of all teachers.
        /// </summary>
        Task<IEnumerable<TeacherDto>> GetTeachersAsync();

        /// <summary>
        /// Gets a single teacher by their unique ID.
        /// </summary>
        Task<TeacherDto?> GetTeacherByIdAsync(string teacherId);

        /// <summary>
        /// Creates a new teacher.
        /// </summary>
        Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto);

        /// <summary>
        /// Updates an existing teacher's information.
        /// </summary>
        Task<bool> UpdateTeacherAsync(string teacherId, UpdateTeacherDto updateTeacherDto);

        /// <summary>
        /// Deletes a teacher by their unique ID.
        /// </summary>
        Task<bool> DeleteTeacherAsync(string teacherId);

        Task<(bool Succeeded, string? newAvatarUrl)> UpdateTeacherAvatarAsync(string teacherId, IFormFile avatarFile);
    }
}