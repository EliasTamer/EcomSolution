using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EcomAPI.Responses;
using System.Net;
using EcomAPI.Services;

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
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Message = "Validaiton failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();

                return BadRequest(response);
            }

            try {
                var userFound = await _usersService.GetUserByEmail(newUser.Email);
                if(userFound != null )
                {   
                    response.Status = (int)HttpStatusCode.BadRequest;
                    response.Message = "User already exists with this email.";
                    return BadRequest(response);
                }
                int id = await _usersService.CreateUser(newUser);

                response.Success = true;
                response.Status = (int)HttpStatusCode.OK;
                response.Message = "User created.";
                response.Data = new { UserCreatedId = id };

                return Ok(response);
            }
            catch(Exception ex) {
                response.Status = (int)HttpStatusCode.InternalServerError;
                response.Message = "An error occurred while creating the user.";
                response.Errors = new List<string> { ex.Message };

                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var response = new ApiResponse();

            if(!ModelState.IsValid)
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

                if(user == null)
                {
                    response.Status = 401;
                    response.Message = "Invalid email or password";
                    return Unauthorized(response);
                }

                if(user.Password != loginRequest.Password)
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

            }
        }
    }
}
