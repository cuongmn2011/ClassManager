// File: src/Infrastructure/Services/LocalFileStorageService.cs
using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalFileStorageService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, subFolder);
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Construct the public URL
            var request = _httpContextAccessor.HttpContext.Request;
            var url = $"{request.Scheme}://{request.Host}/{subFolder}/{uniqueFileName}";
            return url;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            var fileName = Path.GetFileName(filePath);
            var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars", fileName);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}