using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class CreateProductCategoryDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageUrl { get; set; }
    }
}
