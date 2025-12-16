using System.Data;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcomAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IDbConnection _db;
        public UsersService(IDbConnection db)
        {
            _db = db;
        }
        private void InitializeDatabse() {
            
        }
        public CreateUserDTO CreateUser (CreateUserDTO user)
        {
            return user;
        }
    }
}
