using EcomAPI.DTOs;
using EcomAPI.Responses;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace EcomAPI.Services
{
    public class OtpService
    {
        private readonly IDbConnection _db;
        private readonly UsersService _usersService;

        public OtpService(IDbConnection db, UsersService usersService)
        {
            _db = db;
            _usersService = usersService;
        }
        
        public async Task<ServiceResult<string>> GenerateOtp(GenerateOtpDTO otp)
        {
            var userProfileResponse = await _usersService.GetUserProfileByEmail(otp.Email);

            if(userProfileResponse.Success)
            {
                var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
                var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
                var hexFormat = Convert.ToHexString(bytes);

                var createOtpQuery = $"""
                    INSERT INTO OtpCodes
                    VALUES(UserId, CodeHash, Purpose, ExpiresAt)
                    """;

            }

            return ServiceResult<string>.Fail($"{otp.Email} does not exist");
        }
    }
}
