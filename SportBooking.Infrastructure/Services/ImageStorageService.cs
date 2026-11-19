using SportBooking.Application.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace SportBooking.Infrastructure.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string _webRootPath;

        public ImageStorageService(string webRootPath)
        {
            // Đảm bảo luôn có thư mục gốc để lưu ảnh
            _webRootPath = string.IsNullOrWhiteSpace(webRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : webRootPath;
        }

        /// <summary>
        /// Lưu hình ảnh từ Stream và trả về đường dẫn tương đối để lưu vào Database
        /// </summary>
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folderName = "venues", CancellationToken cancellationToken = default)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File hình ảnh không hợp lệ.");

            // 1. Kiểm tra định dạng đuôi file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Chỉ hỗ trợ định dạng ảnh: .jpg, .jpeg, .png, .webp");

            // 2. Tạo thư mục đích nếu chưa tồn tại (VD: wwwroot/uploads/venues)
            var uploadsFolder = Path.Combine(_webRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 3. Tạo tên file độc nhất tránh trùng lặp
            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 4. Lưu file xuống ổ cứng
            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(outputStream, cancellationToken);
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }

       
        public Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Task.CompletedTask;

            var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_webRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}
