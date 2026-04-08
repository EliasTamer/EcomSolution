using System.Data;
using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using EcomAPI.Responses;

namespace EcomAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IDbConnection _db;
        public UsersService(IDbConnection db)
        {
            _db = db;
        }
        public async Task<ServiceResult<int>> CreateUser(CreateUserRequestDTO user)
        {
            var ProfilePhoto = user.ProfilePhoto;

            if (ProfilePhoto.Length > 5 * 1024 * 1024) {
                return ServiceResult<int>.Fail("File size exceeds 5MB limit");
            }

            var allowedTypes = new[] { "image/jpeg", "image/png" };

            if (!allowedTypes.Contains(ProfilePhoto.ContentType))
            {
                return ServiceResult<int>.Fail("Only JPEG and PNG images are allowed");
            }

            var imagePath = Path.Combine("wwwroot/uploads", ProfilePhoto.FileName);
            using var stream = new FileStream(imagePath, FileMode.Create);
            await ProfilePhoto.CopyToAsync(stream);

            User usersParams = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                Role = user.Role,
                Address = user.Address,
                ProfilePhoto = imagePath,
                PhoneNumber = imagePath,
                Country = user.Country,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var sql = @"INSERT INTO Users(FirstName, LastName, Password, Email, Role, Address, ProfilePhoto, PhoneNumber, Country, CreatedAt, UpdatedAt) 
                      VALUES (@FirstName, @LastName, @Password, @Email, @Role, @Address, @ProfilePhoto, @PhoneNumber, @Country, @CreatedAt, @UpdatedAt)
                      SELECT CAST(SCOPE_IDENTITY() as int)";

            int newUserId = await _db.QuerySingleAsync<int>(sql, usersParams);
            return ServiceResult<int>.Ok(newUserId);

        }

        public async Task<ServiceResult<User>> GetUserByEmail(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            var user =  await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
            return ServiceResult<User>.Ok(user);
        }

        public async Task<ServiceResult<UserProfileResponseDTO>> GetUserProfile(int userId)
        {
            var sql = @"SELECT Id, FirstName, Password, LastName, Email, Role, UpdatedAt, CreatedAt, ProfilePhoto, Country, PhoneNumber
                       FROM Users
                       WHERE Id = @Id";

            var profile = await _db.QueryFirstOrDefaultAsync<UserProfileResponseDTO>(sql, new { Id = userId });
            return ServiceResult<UserProfileResponseDTO>.Ok(profile);
        }

        public async Task<ServiceResult<bool>> ChangePassword(ChangePasswordDTO newPasswordRequest)
        {
            var user = await GetUserByEmail(newPasswordRequest.Email);

            if (user == null)
            {
                return ServiceResult<bool>.Fail("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(newPasswordRequest.CurrentPassword, user.Data.Password))
            {
                return ServiceResult<bool>.Fail("Current password is incorrect");
            }

            var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPasswordRequest.NewPassword);

            var sql = "UPDATE Users SET PASSWORD = @Password, UpdatedAt = @UpdatedAt WHERE Email = @Email";

            var rowsAffected = await _db.ExecuteAsync(sql, new
            {
                Password = hashedNewPassword,
                UpdatedAt = DateTime.UtcNow,
                Email = newPasswordRequest.Email
            });

            return ServiceResult<bool>.Ok(rowsAffected > 0);
        }

        public async Task<ServiceResult<bool>> DeleteUser(int userId)
        {
            var user = await GetUserProfile(userId);

            if(user == null)
            {
                return ServiceResult<bool>.Fail("Deletion Failed");
            } else
            {
                var sql = "DELETE FROM Users WHERE Id = @Id";
                var affectedRows = await _db.ExecuteAsync(sql, new { Id = userId });
                return ServiceResult<bool>.Ok(affectedRows > 0);
            }
        }
    }
}
