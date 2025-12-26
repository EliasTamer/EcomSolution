using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EcomAPI.Responses;
using System.Net;

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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDTO newUser)
        {
            var response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validaiton failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();

                return BadRequest(response);
            }

            try
            {
                var userFound = await _usersService.GetUserByEmail(newUser.Email);
                if (userFound != null)
                {
                    response.Status = 400;
                    response.Message = "User already exists with this email.";
                    return BadRequest(response);
                }
                int id = await _usersService.CreateUser(newUser);

                response.Success = true;
                response.Status = 200;
                response.Message = "User created.";
                response.Data = new { UserCreatedId = id };

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = "An error occurred while creating the user.";
                response.Errors = new List<string> { ex.Message };

                return StatusCode(500, response);
            }
        }
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

            try
            {
                var user = await _usersService.GetUserByEmail(loginRequest.Email);

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
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = "An error occurred during login.";
                response.Errors = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }
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

            try
            {
                var result = await _usersService.ChangePassword(changePasswordRequest);

                if (!result.Success)
                {
                    response.Status = 400;
                    response.Message = result.Message;
                    return BadRequest(response);
                }

                response.Success = true;
                response.Status = 200;
                response.Message = result.Message;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = "An error occured";
                response.Errors = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }
        [HttpGet("GetUserProfile/{userId}")]
        public async Task<IActionResult> GetUserProfile([FromRoute] int userId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var userProfile = await _usersService.GetUserProfile(userId);
                if (userProfile == null)
                {
                    response.Status = 404;
                    response.Message = "User not found";
                    return NotFound(response);
                }
                response.Success = true;
                response.Status = 200;
                response.Message = "User profile retrieved successfully";
                response.Data = userProfile;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = 500;
                response.Message = "An error occurred while retrieving the user profile.";
                response.Errors = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }
    }
}
