using EcomAPI.Entities;
using EcomAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcomAPI.Controllers
{
    [ApiController]
    [Route("api/UsersAuth")]
    public class UsersController : ControllerBase
    {
        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            UserEntity user = new UserEntity()
            {
                Id = 1,
                FirstName = "Elias",
                LastName = "Tamer",
                Email = "eliastamer@gmail.com",
                Role = "admin"
            };
            return Ok(user);
        }
    }
}
