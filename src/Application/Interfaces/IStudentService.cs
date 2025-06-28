using Application.DTOs.Students;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IStudentService
    {
        /// <summary>
        /// Gets a list of all students.
        /// </summary>
        /// <returns>A collection of StudentDto.</returns>
        Task<IEnumerable<StudentDto>> GetStudentsAsync();

        /// <summary>
        /// Gets a single student by their unique ID.
        /// </summary>
        /// <param name="studentId">The ID of the student to retrieve.</param>
        /// <returns>A StudentDto object if found; otherwise, null.</returns>
        Task<StudentDto?> GetStudentByIdAsync(string studentId);

        /// <summary>
        /// Creates a new student based on the provided data.
        /// </summary>
        /// <param name="createStudentDto">The data transfer object containing the information for the new student.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created student DTO, including the generated ID.</returns>
        Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto);

        /// <summary>
        /// Updates an existing student's information.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student to update.</param>
        /// <param name="updateStudentDto">The data transfer object containing the updated information for the student.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the update was successful, and false if the student was not found.</returns>
        Task<bool> UpdateStudentAsync(string studentId, UpdateStudentDto updateStudentDto);

        /// <summary>
        /// Deletes a student by their unique ID.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the deletion was successful, and false if the student was not found.</returns>
        Task<bool> DeleteStudentAsync(string studentId);

        Task<(bool Succeeded, string? newAvatarUrl)> UpdateStudentAvatarAsync(string studentId, IFormFile avatarFile);
    }
}