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
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        [HttpPost("CreateUser")]
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
    }
}
