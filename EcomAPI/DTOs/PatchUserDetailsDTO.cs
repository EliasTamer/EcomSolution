
namespace EcomAPI.DTOs
{
    public class PatchUserDetailsDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
        public string? Country { get; set; }
    }
}
