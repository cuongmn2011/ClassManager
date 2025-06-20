// File: src/Api/Services/MockTeacherService.cs
// English comments as requested.

using Application.DTOs.Teachers;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
    public class MockTeacherService : ITeacherService
    {
        // Create a hard-coded list of teachers for mocking purposes.
        private readonly List<TeacherDto> _teachers = new()
        {
            new TeacherDto
            {
                TeacherId = "TEA001",
                UserId = "user-guid-101",
                FullName = "Phan Thị Ngọc",
                PhoneNumber = "0987654321",
                DateOfBirth = new DateTime(1990, 1, 20),
                Address = "111 Đường Sư Phạm, Quận Cầu Giấy, Hà Nội",
                Gender = "Nữ",
                Status = "Đang làm việc",
                Specialization = "Toán Cao Cấp"
            },
            new TeacherDto
            {
                TeacherId = "TEA002",
                UserId = "user-guid-102",
                FullName = "Hoàng Trung Dũng",
                PhoneNumber = "0912345678",
                DateOfBirth = new DateTime(1985, 11, 5),
                Address = "222 Đường Khoa Học, Quận 1, TP. HCM",
                Gender = "Nam",
                Status = "Đang làm việc",
                Specialization = "Vật Lý Đại Cương"
            }
        };

        public Task<IEnumerable<TeacherDto>> GetTeachersAsync()
        {
            return Task.FromResult<IEnumerable<TeacherDto>>(_teachers.Where(t => t.Status != "Đã xóa"));
        }

        public Task<TeacherDto?> GetTeacherByIdAsync(string teacherId)
        {
            var teacher = _teachers.FirstOrDefault(t => t.TeacherId == teacherId && t.Status != "Đã xóa");
            return Task.FromResult(teacher);
        }

        public Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto)
        {
            var newTeacher = new TeacherDto
            {
                TeacherId = $"TEA{new Random().Next(100, 999)}",
                UserId = $"user-guid-{new Random().Next(200, 999)}",
                FullName = createTeacherDto.FullName,
                PhoneNumber = createTeacherDto.PhoneNumber,
                DateOfBirth = createTeacherDto.DateOfBirth,
                Address = createTeacherDto.Address,
                Gender = createTeacherDto.Gender,
                Specialization = createTeacherDto.Specialization,
                Status = "Đang làm việc"
            };
            _teachers.Add(newTeacher);
            return Task.FromResult(newTeacher);
        }

        public Task<bool> UpdateTeacherAsync(string teacherId, UpdateTeacherDto updateTeacherDto)
        {
            var existingTeacher = _teachers.FirstOrDefault(t => t.TeacherId == teacherId);
            if (existingTeacher == null) return Task.FromResult(false);

            existingTeacher.FullName = updateTeacherDto.FullName;
            existingTeacher.PhoneNumber = updateTeacherDto.PhoneNumber;
            existingTeacher.DateOfBirth = updateTeacherDto.DateOfBirth;
            existingTeacher.Address = updateTeacherDto.Address;
            existingTeacher.Gender = updateTeacherDto.Gender;
            existingTeacher.Specialization = updateTeacherDto.Specialization;
            existingTeacher.Status = updateTeacherDto.Status;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteTeacherAsync(string teacherId)
        {
            var teacherToDelete = _teachers.FirstOrDefault(t => t.TeacherId == teacherId);
            if (teacherToDelete == null) return Task.FromResult(false);

            teacherToDelete.Status = "Đã xóa"; // Soft delete
            return Task.FromResult(true);
        }
    }
}