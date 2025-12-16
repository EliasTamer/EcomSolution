using EcomAPI.DTOs;
using EcomAPI.Entities;

namespace EcomAPI.Interfaces
{
    public interface IUsersService
    {
        CreateUserDTO CreateUser(CreateUserDTO user);
    }
}
