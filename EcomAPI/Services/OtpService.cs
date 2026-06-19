using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
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
        
        public async Task<ServiceResult<bool>> GenerateOtp(GenerateOtpDTO otp)
        {
            var userProfileResponse = await _usersService.GetUserProfileByEmail(otp.Email);

            if(userProfileResponse.Success && userProfileResponse.Data != null)
            {
                var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
                var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
                var hexFormatOtp = Convert.ToHexString(bytes);

                var sql = """
                    INSERT INTO OtpCodes(UserId, CodeHash, Purpose, ExpiresAt)
                    VALUES(@UserId, @CodeHash, @Purpose, @ExpiresAt)
                    """;

                await _db.ExecuteAsync(sql, new { UserId = userProfileResponse.Data.Id, CodeHash = hexFormatOtp, Purpose = otp.Purpose.ToString(), ExpiresAt = DateTime.UtcNow.AddMinutes(5) });
                Console.WriteLine($"[DEV] OTP for {otp.Email}: {code}"); // remove once email is wired
            }

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ValidateOtp(ValidateOtpDTO otp)
        {
            var userProfileResponse = await _usersService.GetUserProfileByEmail(otp.Email);

            if(userProfileResponse.Success && userProfileResponse.Data != null)
            {
                var sql = """
                SELECT TOP 1 *
                FROM OtpCodes
                Where UserId = @UserId
                      AND Purpose = @Purpose
                      AND ConsumedAt IS NULL 
                      AND ExpiresAt > @Now
                ORDER BY CreatedAt DESC
                """;

                var row = await _db.QueryFirstOrDefaultAsync<Otp>(sql, new { UserId = userProfileResponse.Data.Id, Purpose = otp.Purpose.ToString(), Now = DateTime.UtcNow });

                if(row == null)
                {
                    return ServiceResult<bool>.Fail("Invalid or expired code");
                }

                if(row.AttemptCount > 5)
                {
                    return ServiceResult<bool>.Fail("Too many attemps");
                }

                var incomingHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(otp.Otp)));
                var matches = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(row.CodeHash),
                Encoding.UTF8.GetBytes(incomingHash));

                if(!matches)
                {
                    await _db.ExecuteAsync("UPDATE OtpCodes SET AttemptCount =  AttemptCount + 1 Where Id = @Id", new {Id = row.Id});
                    return ServiceResult<bool>.Fail("Invalid or expired otp");
                }

                await _db.ExecuteAsync("UPDATE OtpCodes SET ConsumedAt = @Now Where Id = @Id", new {Now = DateTime.UtcNow, Id = row.Id});

                return ServiceResult<bool>.Ok(true);
            }

            return ServiceResult<bool>.Fail("Validation Failed");

        }
    }
}
