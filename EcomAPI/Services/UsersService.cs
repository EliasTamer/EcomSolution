using System.Data;
using Dapper;
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
        public async Task<int> CreateUser(CreateUserRequestDTO user)
        {
            User usersParams = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = user.Password,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var sql = @"INSERT INTO Users(FirstName, LastName, Password, Email, Role, CreatedAt, UpdatedAt) 
                      VALUES (@FirstName, @LastName, @Password, @Email, @Role, @CreatedAt, @UpdatedAt)
                      SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.QuerySingleAsync<int>(sql, usersParams);

        }
    }
}
