// File: src/Api/Services/MockStudentService.cs
using Application.DTOs.Students;
using Application.Interfaces;

namespace Api.Services
{
    public class MockStudentService : IStudentService
    {
        private readonly List<StudentDto> _students = new()
        {
            new StudentDto
            {
                StudentId = "STU001",
                UserId = "user-guid-001",
                FullName = "Nguyễn Văn An",
                PhoneNumber = "0901234567",
                DateOfBirth = new DateTime(2010, 5, 15),
                AvatarUrl = "https://example.com/avatars/an.png",
                Address = "123 Đường ABC, Quận 1, TP. HCM",
                Gender = "Nam",
                Status = "Đang học"
            },
            new StudentDto
            {
                StudentId = "STU002",
                UserId = "user-guid-002",
                FullName = "Trần Thị Bình",
                PhoneNumber = "0907654321",
                DateOfBirth = new DateTime(2011, 8, 22),
                AvatarUrl = null, // Can be null
                Address = "456 Đường XYZ, Quận 3, TP. HCM",
                Gender = "Nữ",
                Status = "Đang học"
            },
            new StudentDto
            {
                StudentId = "STU003",
                UserId = "user-guid-003",
                FullName = "Lê Văn Cường",
                PhoneNumber = "0912345678",
                DateOfBirth = new DateTime(2010, 2, 10),
                AvatarUrl = "https://example.com/avatars/cuong.png",
                Address = "789 Đường KLM, Quận 5, TP. HCM",
                Gender = "Nam",
                Status = "Tạm nghỉ"
            }
        };

        public Task<IEnumerable<StudentDto>> GetStudentsAsync()
        {
            return Task.FromResult<IEnumerable<StudentDto>>(_students.Where(s => s.Status != "Đã xóa"));
        }

        public Task<StudentDto?> GetStudentByIdAsync(string studentId)
        {
            var student = _students.FirstOrDefault(s => s.StudentId == studentId && s.Status != "Đã xóa");
            return Task.FromResult(student);
        }

        public Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
        {
            var newStudent = new StudentDto
            {
                // In a real app, ID would be a GUID. Here we just generate a random one.
                StudentId = $"STU{new Random().Next(100, 999)}",
                UserId = $"user-guid-{new Random().Next(100, 999)}",
                FullName = createStudentDto.FullName,
                PhoneNumber = createStudentDto.PhoneNumber,
                DateOfBirth = createStudentDto.DateOfBirth,
                Address = createStudentDto.Address,
                Gender = createStudentDto.Gender,
                Status = "Đang học" // Default status
            };
            _students.Add(newStudent);
            return Task.FromResult(newStudent);
        }

        public Task<bool> UpdateStudentAsync(string studentId, UpdateStudentDto updateStudentDto)
        {
            var existingStudent = _students.FirstOrDefault(s => s.StudentId == studentId);
            if (existingStudent == null)
            {
                return Task.FromResult(false); // Not found
            }

            // Update properties
            existingStudent.FullName = updateStudentDto.FullName;
            existingStudent.PhoneNumber = updateStudentDto.PhoneNumber;
            existingStudent.DateOfBirth = updateStudentDto.DateOfBirth;
            existingStudent.Address = updateStudentDto.Address;
            existingStudent.Gender = updateStudentDto.Gender;
            existingStudent.Status = updateStudentDto.Status;

            return Task.FromResult(true); // Success
        }

        public Task<bool> DeleteStudentAsync(string studentId)
        {
            var studentToDelete = _students.FirstOrDefault(s => s.StudentId == studentId);
            if (studentToDelete == null)
            {
                return Task.FromResult(false); // Not found
            }

            // Instead of removing, we can implement soft delete by changing status
            // This aligns with requirement STU-005 to change student status
            studentToDelete.Status = "Đã xóa";
            // _students.Remove(studentToDelete); // This would be a hard delete

            return Task.FromResult(true); // Success
        }
    }
}