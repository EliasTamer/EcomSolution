using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IFileService
    {
        Task<ServiceResult<string>> StoreFile(IFormFile file);
        ServiceResult<bool> DeleteFile(string path);
    }
}
