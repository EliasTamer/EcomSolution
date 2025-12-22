using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
