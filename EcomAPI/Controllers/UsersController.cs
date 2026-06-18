using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EcomAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace EcomAPI.Controllers
{
    [ApiController]
    [Route("api/UsersAuth")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IJwtService _jwtService;
        public UsersController(IUsersService usersService, IJwtService jwtService)
        {
            _usersService = usersService;
            _jwtService = jwtService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequestDTO newUser)
        {
            ApiResponse response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validaiton failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
                return BadRequest(response);
            }

            var getUserResult = await _usersService.GetUserByEmail(newUser.Email);
            if (getUserResult.Data != null)
            {
                response.Status = 400;
                response.Message = "User already exists with this email.";
                return BadRequest(response);
            }

            var createUserResult = await _usersService.CreateUser(newUser);

            if (!createUserResult.Success)
            {
                response.Status = 400;
                response.Message = createUserResult.Message;
                return BadRequest(response);
            }

            response.Success = true;
            response.Status = 200;
            response.Message = "User created.";
            response.Data = createUserResult.Data;
            return Ok(response);
        }

        [EnableRateLimiting("fixed")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(response);
            }

            var getUserResult = await _usersService.GetUserByEmail(loginRequest.Email);
            var user = getUserResult.Data;

            if (user == null)
            {
                response.Status = 401;
                response.Message = "Invalid email or password";
                return Unauthorized(response);
            }

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                response.Status = 401;
                response.Message = "Invalid email or password";
                return Unauthorized(response);
            }

            var token = _jwtService.GenerateToken(user);
            response.Success = true;
            response.Status = 200;
            response.Message = "Login succesful.";
            response.Data = new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                }
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordRequest)
        {
            ApiResponse response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
                return BadRequest(response);
            }

            var changePasswordResult = await _usersService.ChangePassword(changePasswordRequest);

            if (!changePasswordResult.Success)
            {
                response.Status = 400;
                response.Message = changePasswordResult.Message;
                return BadRequest(response);
            }

            response.Success = true;
            response.Status = 200;
            response.Message = changePasswordResult.Message;
            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetUserProfile/{userId}")]
        public async Task<IActionResult> GetUserProfile([FromRoute] int userId)
        {
            var response = new ApiResponse();

            var getUserProfileResult = await _usersService.GetUserProfileById(userId);

            if (getUserProfileResult.Data == null)
            {
                response.Status = 404;
                response.Message = "User not found";
                return NotFound(response);
            }

            response.Success = true;
            response.Status = 200;
            response.Message = "User profile retrieved successfully";
            response.Data = getUserProfileResult.Data;
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {
            ApiResponse response = new ApiResponse();
            var deleteUserResult = await _usersService.DeleteUser(userId);

            if (!deleteUserResult.Success)
            {
                response.Status = 400;
                response.Message = "User deletion failed.";
                return BadRequest(response);
            }

            response.Success = true;
            response.Message = "User deleted successfuly";
            return Ok(response);
        }
    }
}