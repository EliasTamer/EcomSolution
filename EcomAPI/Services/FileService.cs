using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EcomAPI.Interfaces;
using EcomAPI.Responses;

namespace EcomAPI.Services
{
    public class FileService : IFileService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string[] _allowedExtensions;
        private readonly int _maxSize;

        public FileService(BlobServiceClient blobServiceClient, string containerName, string[] allowedExtensions, int maxSize)
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
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

            await _containerClient.CreateIfNotExistsAsync();

            var safeName = $"{Guid.NewGuid()}{extension}";
            var blobClient = _containerClient.GetBlobClient(safeName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            };

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, options);

            return ServiceResult<string>.Ok(safeName);

        }

        public async Task<ServiceResult<bool>> DeleteFile(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var response = await blobClient.DeleteIfExistsAsync();

            return response.Value ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.Fail("File not found");
        }
    }
}
