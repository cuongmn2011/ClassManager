// File: src/Infrastructure/Services/StudentService.cs
using Application.DTOs.Students;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IFileStorageService _fileStorageService;
        public StudentService(ApplicationDbContext context,
        UserManager<User> userManager, 
        IFileStorageService fileStorageService)
        {
            _context = context;
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsAsync()
        {
            return await _context.Students
                .Where(s => !s.IsDeleted)
                .Select(s => new StudentDto
                {
                    StudentId = s.Id.ToString(),
                    UserId = s.UserId,
                    FullName = s.FullName,
                    PhoneNumber = s.PhoneNumber,
                    DateOfBirth = s.DateOfBirth,
                    AvatarUrl = s.AvatarUrl,
                    Address = s.Address,
                    Gender = s.Gender,
                    Status = s.Status
                })
                .ToListAsync();
        }

        public async Task<StudentDto?> GetStudentByIdAsync(string studentId)
        {
            if (!int.TryParse(studentId, out int id))
            {
                return null;
            }

            var student = await _context.Students
                .Where(s => s.Id == id && !s.IsDeleted)
                .Select(s => new StudentDto
                {
                    StudentId = s.Id.ToString(),
                    UserId = s.UserId,
                    FullName = s.FullName,
                    PhoneNumber = s.PhoneNumber,
                    DateOfBirth = s.DateOfBirth,
                    AvatarUrl = s.AvatarUrl,
                    Address = s.Address,
                    Gender = s.Gender,
                    Status = s.Status
                })
                .FirstOrDefaultAsync();

            return student;
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
        {
            // 1. Check if username already exists
            var existingUser = await _userManager.FindByNameAsync(createStudentDto.UserName);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            // 2. Create a new User account
            var newUser = new User
            {
                UserName = createStudentDto.UserName, // <-- Lấy từ DTO
                Email = createStudentDto.Email,
                FullName = createStudentDto.FullName,
                PhoneNumber = createStudentDto.PhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            // Use the new default password
            var userCreationResult = await _userManager.CreateAsync(newUser, "123456");

            if (!userCreationResult.Succeeded)
            {
                throw new Exception(string.Join(", ", userCreationResult.Errors.Select(e => e.Description)));
            }

            // 3. Assign the "Student" role
            await _userManager.AddToRoleAsync(newUser, "Student");

            // 4. Create the Student profile
            var student = new Student
            {
                UserId = newUser.Id,
                FullName = createStudentDto.FullName,
                PhoneNumber = createStudentDto.PhoneNumber,
                DateOfBirth = createStudentDto.DateOfBirth,
                Address = createStudentDto.Address,
                Gender = createStudentDto.Gender,
                Status = "Đang học",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            // 5. Return the newly created student's data
            return (await GetStudentByIdAsync(student.Id.ToString()))!;
        }

        public async Task<bool> UpdateStudentAsync(string studentId, UpdateStudentDto updateStudentDto)
        {
            if (!int.TryParse(studentId, out int id))
            {
                return false;
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null || student.IsDeleted)
            {
                return false;
            }

            student.FullName = updateStudentDto.FullName;
            student.PhoneNumber = updateStudentDto.PhoneNumber;
            student.DateOfBirth = updateStudentDto.DateOfBirth;
            student.Address = updateStudentDto.Address;
            student.Gender = updateStudentDto.Gender;
            student.Status = updateStudentDto.Status;
            student.UpdatedAt = DateTime.UtcNow;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStudentAsync(string studentId)
        {
            if (!int.TryParse(studentId, out int id))
            {
                return false;
            }
            
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            student.IsDeleted = true; // Soft delete
            student.Status = "Đã xóa";
            student.UpdatedAt = DateTime.UtcNow;

            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<(bool Succeeded, string? newAvatarUrl)> UpdateStudentAvatarAsync(string studentId, IFormFile avatarFile)
        {
            if (!int.TryParse(studentId, out int id))
            {
                return (false, null);
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return (false, null);
            }

            // Xóa avatar cũ nếu có
            if (!string.IsNullOrEmpty(student.AvatarUrl))
            {
                _fileStorageService.DeleteFile(student.AvatarUrl);
            }

            // Lưu avatar mới và lấy URL. Thay đổi thư mục con thành "uploads/students"
            var newAvatarUrl = await _fileStorageService.SaveFileAsync(avatarFile, "uploads/students");

            // Cập nhật lại record trong database
            student.AvatarUrl = newAvatarUrl;
            student.UpdatedAt = DateTime.UtcNow;

            if (student.User != null)
            {
                student.User.AvatarUrl = newAvatarUrl;
                await _userManager.UpdateAsync(student.User);
            }

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return (true, newAvatarUrl);
        }

            // Các phương thức khác sẽ được thêm sau khi có logic cụ thể hơn
            // Tạm thời để trống hoặc trả về giá trị mặc định.
        }
}