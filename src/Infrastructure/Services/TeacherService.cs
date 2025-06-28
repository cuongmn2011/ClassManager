// File: src/Infrastructure/Services/TeacherService.cs
using Application.DTOs.Teachers;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http; // Add this
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        private readonly IFileStorageService _fileStorageService; // Inject for avatar upload

        public TeacherService(ApplicationDbContext context,
        UserManager<User> userManager, 
        IFileStorageService fileStorageService)
        {
            _context = context;
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<TeacherDto>> GetTeachersAsync()
        {
            return await _context.Teachers
                .Where(t => !t.IsDeleted) // Assuming Teacher entity has IsDeleted
                .Select(t => new TeacherDto
                {
                    TeacherId = t.Id.ToString(),
                    UserId = t.UserId,
                    FullName = t.FullName,
                    PhoneNumber = t.PhoneNumber,
                    DateOfBirth = t.DateOfBirth,
                    AvatarUrl = t.AvatarUrl,
                    Address = t.Address,
                    Gender = t.Gender,
                    Status = t.Status,
                    Specialization = t.Specialization
                })
                .ToListAsync();
        }

        public async Task<TeacherDto?> GetTeacherByIdAsync(string teacherId)
        {
            if (!int.TryParse(teacherId, out int id)) return null;

            return await _context.Teachers
                .Where(t => t.Id == id && !t.IsDeleted)
                .Select(t => new TeacherDto { /*...mapping properties...*/ })
                .FirstOrDefaultAsync();
        }

        public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto)
        {
            // 1. Check if user exists
            var existingUser = await _userManager.FindByNameAsync(createTeacherDto.UserName);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            // 2. Create new User
            var newUser = new User
            {
                UserName = createTeacherDto.UserName, // <-- Lấy từ DTO
                Email = createTeacherDto.Email,
                FullName = createTeacherDto.FullName,
                PhoneNumber = createTeacherDto.PhoneNumber,
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

            // 3. Assign "Teacher" role
            await _userManager.AddToRoleAsync(newUser, "Teacher");

            // 4. Create Teacher profile
            var teacher = new Teacher
            {
                UserId = newUser.Id,
                FullName = createTeacherDto.FullName,
                PhoneNumber = createTeacherDto.PhoneNumber,
                DateOfBirth = createTeacherDto.DateOfBirth,
                Address = createTeacherDto.Address,
                Gender = createTeacherDto.Gender,
                Specialization = createTeacherDto.Specialization,
                Status = "Đang làm việc",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            // 5. Return DTO
            return (await GetTeacherByIdAsync(teacher.Id.ToString()))!;
        }

        public async Task<bool> UpdateTeacherAsync(string teacherId, UpdateTeacherDto updateTeacherDto)
        {
            if (!int.TryParse(teacherId, out int id)) return false;

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null || teacher.IsDeleted) return false;

            teacher.FullName = updateTeacherDto.FullName;
            teacher.PhoneNumber = updateTeacherDto.PhoneNumber;
            teacher.DateOfBirth = updateTeacherDto.DateOfBirth;
            teacher.Address = updateTeacherDto.Address;
            teacher.Gender = updateTeacherDto.Gender;
            teacher.Specialization = updateTeacherDto.Specialization;
            teacher.Status = updateTeacherDto.Status;
            teacher.UpdatedAt = DateTime.UtcNow;

            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTeacherAsync(string teacherId)
        {
            if (!int.TryParse(teacherId, out int id)) return false;

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return false;

            teacher.IsDeleted = true;
            teacher.Status = "Đã xóa";
            teacher.UpdatedAt = DateTime.UtcNow;

            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Succeeded, string? newAvatarUrl)> UpdateTeacherAvatarAsync(string teacherId, IFormFile avatarFile)
        {
            if (!int.TryParse(teacherId, out int id)) return (false, null);

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return (false, null);

            if (!string.IsNullOrEmpty(teacher.AvatarUrl))
            {
                _fileStorageService.DeleteFile(teacher.AvatarUrl);
            }

            var newAvatarUrl = await _fileStorageService.SaveFileAsync(avatarFile, "uploads/teachers");

            teacher.AvatarUrl = newAvatarUrl;
            teacher.UpdatedAt = DateTime.UtcNow;

            if (teacher.User != null)
            {
                teacher.User.AvatarUrl = newAvatarUrl;
                await _userManager.UpdateAsync(teacher.User);
            }

            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();

            return (true, newAvatarUrl);
        }
    }
}