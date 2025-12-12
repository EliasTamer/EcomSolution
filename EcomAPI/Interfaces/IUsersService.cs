using EcomAPI.DTOs;
using EcomAPI.Entities;

namespace EcomAPI.Interfaces
{
    public interface IUsersService
    {
        Task<int> CreateUser(CreateUserDTO user);
        Task<User?> GetUserDetails(int id);
    }
}
