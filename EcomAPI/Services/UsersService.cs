using System.Data;
using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;

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
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
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

        public async Task<User?> GetUserByEmail(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<PasswordChangeResult> ChangePassword(ChangePasswordDTO newPasswordRequest)
        {
            var user = await GetUserByEmail(newPasswordRequest.Email);
            PasswordChangeResult result = new PasswordChangeResult();

            if (user == null)
            {
                result.Message = "User not found";
                return result;
            }

            if (!BCrypt.Net.BCrypt.Verify(newPasswordRequest.CurrentPassword, user.Password))
            {
                result.Message = "Current password is incorrect";
                return result;
            }

            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPasswordRequest.NewPassword);

            var sql = "UPDATE Users SET PASSWORD = @Password, UpdatedAt = @UpdatedAt WHERE Email = @Email";

            var rowsAffected = await _db.ExecuteAsync(sql, new
            {
                Password = hashedNewPassword,
                UpdatedAt = DateTime.UtcNow,
                Email = newPasswordRequest.Email
            });

            if(rowsAffected == 0)
            {
                result.Message = "Password update failed";
                return result;
            }

            result.Success = true;
            result.Message = "Password updated successfully";
            return result;
        }
    }
}
