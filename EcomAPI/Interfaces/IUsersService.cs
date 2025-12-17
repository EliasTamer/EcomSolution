using EcomAPI.DTOs;
using EcomAPI.Entities;

namespace EcomAPI.Interfaces
{
    public interface IUsersService
    {
        Task<int> CreateUser(CreateUserRequestDTO user);
    }
}
