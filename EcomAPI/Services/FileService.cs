using EcomAPI.Interfaces;
using EcomAPI.Responses;

namespace EcomAPI.Services
{
    public class FileService : IFileService
    {
        private readonly string _folderPath;
        private readonly string[] _allowedExtensions;
        private readonly int _maxSize;

        public FileService(string folderPath, string[] allowedExtensions, int maxSize)
        {
            _folderPath = folderPath;
            _allowedExtensions = allowedExtensions;
            _maxSize = maxSize;
        }

        public async Task<ServiceResult<string>> StoreFile(IFormFile file)
        {

            if (file.Length == 0)
            {
                return ServiceResult<string>.Fail("File is empty");
            }
             
            var allowedMaxSizeInBytes = _maxSize * 1024 * 1024;

            if (file.Length > allowedMaxSizeInBytes)
            {
                return ServiceResult<string>.Fail($"File size exceeds {_maxSize}MB limit");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                string message = $"Only these file types are allowed {string.Join(", ", _allowedExtensions)}";
                return ServiceResult<string>.Fail(message);
            }

            var folderPath = Path.Combine("wwwroot", _folderPath);
            Directory.CreateDirectory(folderPath);

            var safeName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(folderPath, safeName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return ServiceResult<string>.Ok(safeName);
        }

        public ServiceResult<bool> DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return ServiceResult<bool>.Ok(true);
            }
            return ServiceResult<bool>.Fail("File not found");
        }
    }
}
