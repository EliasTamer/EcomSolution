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
        private readonly IFileService _fileService;

        public UsersService(IDbConnection db, [FromKeyedServices("userPhotos")] IFileService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        public async Task<ServiceResult<int>> CreateUser(CreateUserRequestDTO user)
        {
            var userPhoto = user.ProfilePhoto;
            var imagePath = string.Empty;

            if (userPhoto != null)
            {
                var storeImageResult = await _fileService.StoreFile(userPhoto);
                if (storeImageResult.Success)
                {
                    imagePath = storeImageResult.Data;
                }
                else
                {
                    return ServiceResult<int>.Fail(storeImageResult.Message);
                }
            }

            User usersParams = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Email = user.Email,
                Role = user.Role,
                Address = user.Address,
                ProfilePhoto = imagePath,
                PhoneNumber = user.PhoneNumber,
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
            var user = await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });

            if (user == null)
            {
                return ServiceResult<User>.Fail("User not found");
            }

            return ServiceResult<User>.Ok(user);
        }

        public async Task<ServiceResult<UserProfileResponseDTO>> GetUserProfile(int userId)
        {
            var sql = @"SELECT Id, FirstName, LastName, Email, Role, UpdatedAt, CreatedAt, ProfilePhoto, Country, PhoneNumber
                       FROM Users
                       WHERE Id = @Id";

            var profile = await _db.QueryFirstOrDefaultAsync<UserProfileResponseDTO>(sql, new { Id = userId });

            if(profile == null)
            {
                return ServiceResult<UserProfileResponseDTO>.Fail("Profile not found");
            }

            return ServiceResult<UserProfileResponseDTO>.Ok(profile);
        }

        public async Task<ServiceResult<bool>> ChangePassword(ChangePasswordDTO newPasswordRequest)
        {
            var user = await GetUserByEmail(newPasswordRequest.Email);

            if (user.Data == null)
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

            if (user == null)
            {
                return ServiceResult<bool>.Fail("Deletion Failed");
            }

            var sql = "DELETE FROM Users WHERE Id = @Id";
            var affectedRows = await _db.ExecuteAsync(sql, new { Id = userId });
            return ServiceResult<bool>.Ok(affectedRows > 0);
        }

        public async Task<ServiceResult<bool>> PatchUserDetails(PatchUserDetailsDTO user)
        {
            var userPhoto = user.ProfilePhoto;
            var imagePath = string.Empty;

            if (userPhoto != null)
            {
                var storeImageResult = await _fileService.StoreFile(userPhoto);
                if (storeImageResult.Success)
                {
                    imagePath = storeImageResult.Data;
                }
                else
                {
                    return ServiceResult<bool>.Fail(storeImageResult.Message);
                }

                var sql = @"UPDATE Users
                            SET FirstName = COALESCE(@FirstName, FirstName),
                                LastName = COALESCE(@LastName, LastName),
                                Address = COALESCE(@Address, Address),
                                Role = COALESCE(@Role, Role),
                                Country = COALESCE(@Country, Country),
                                ProfilePhoto = COALESCE(@ProfilePhoto, ProfilePhoto),
                                UpdatedAt = @UpdatedAt
                            OUTPUT deleted.Id as Id, deleted.ProfilePhoto as OldPhoto
                            WHERE Id = @Id";

                var row = await _db.QuerySingleOrDefaultAsync<(int Id, string? OldPhoto)>(sql, new
                {
                    user.FirstName,
                    user.LastName,
                    user.Address,
                    user.Role,
                    user.Country,
                    ProfilePhoto = userPhoto,
                    UpdatedAt = DateTime.Now,
                });

                if (row.Id == 0)
                {
                    if(userPhoto != null)
                    {

                    }
                    return ServiceResult<bool>.Fail("User not found");
                }

                if (userPhoto != null && !string.IsNullOrEmpty(row.OldPhoto))
                {

                }

                return ServiceResult<bool>.Ok(true);
            }

        }
    }
}