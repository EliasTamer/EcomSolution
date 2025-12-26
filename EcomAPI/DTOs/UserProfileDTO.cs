namespace EcomAPI.DTOs
{
    public class UserProfileResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string ProfilePhoto { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
