using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IUsersService
    {
        Task<ServiceResult<int>> CreateUser(CreateUserRequestDTO user);
        Task<ServiceResult<User>> GetUserByEmail(string email);
        Task<ServiceResult<bool>> ChangePassword(ChangePasswordDTO newPassword);
        Task<ServiceResult<UserProfileResponseDTO>> GetUserProfileById(int userId);
        Task<ServiceResult<UserProfileResponseDTO>> GetUserProfileByEmail(string email);
        Task<ServiceResult<bool>> DeleteUser(int userId);
        Task<ServiceResult<bool>> PatchUserDetails(PatchUserDetailsDTO user);
    }
}
