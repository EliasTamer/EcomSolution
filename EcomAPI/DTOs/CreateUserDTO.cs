using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class CreateUserRequestDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        public IFormFile ProfilePhoto { get; set; }
        public string Country { get; set; } = string.Empty;

    }
}