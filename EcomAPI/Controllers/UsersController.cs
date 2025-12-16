using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using EcomAPI.Services;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("GetUser")]
        public IActionResult CreateUser()
        {
            CreateUserDTO user = new CreateUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Password = "Password123",
                Email = ""
            };
            return Ok(user);
        }
    }
}
