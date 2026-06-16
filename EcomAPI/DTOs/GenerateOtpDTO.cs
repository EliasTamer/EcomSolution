using EcomAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class GenerateOtpDTO
    {
        [Required]
        [EmailAddress]
        public string Email {  get; set; } = string.Empty;
        [Required]
        [EnumDataType(typeof(OtpPurpose))]
        public OtpPurpose? Purpose { get; set; }
    }
}
