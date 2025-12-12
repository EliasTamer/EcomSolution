using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcomAPI.Services
{
    public class UsersService : IUsersService
    {
        public Task<int> CreateUser(CreateUserDTO user)
        {

        }
        public Task<User?> GetUserDetails(int id) { 
        }

    }
}
