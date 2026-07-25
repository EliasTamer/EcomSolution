using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IFileService
    {
        Task<ServiceResult<string>> StoreFile(IFormFile file);
        Task<ServiceResult<bool>> DeleteFile(string path);
    }
}
