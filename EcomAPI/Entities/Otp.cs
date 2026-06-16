namespace EcomAPI.Entities
{
    public enum OtpPurpose
    {
        Login,
        PasswordReset,
    }

    public class Otp
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CodeHash { get; set; } = string.Empty;
        public OtpPurpose Purpose { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? ConsumedAt { get; set; }
        public int AttemptCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
