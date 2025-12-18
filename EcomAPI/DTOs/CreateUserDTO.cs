using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class CreateUserRequestDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
    }
}