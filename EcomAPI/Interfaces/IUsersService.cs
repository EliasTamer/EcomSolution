using EcomAPI.DTOs;

namespace EcomAPI.Interfaces
{
    public interface IUsersService
    {
        Task<int> CreateUser(CreateUserRequestDTO user);
        Task<UserResponseDTO?> GetUserByEmail(string email);
    }
}
