using EcomAPI.DTOs;
using EcomAPI.Entities;

namespace EcomAPI.Interfaces
{
    public interface IUsersService
    {
        Task<int> CreateUser(CreateUserRequestDTO user);
        Task<User?> GetUserByEmail(string email);
        Task<PasswordChangeResult> ChangePassword(ChangePasswordDTO newPassword);
    }
}
