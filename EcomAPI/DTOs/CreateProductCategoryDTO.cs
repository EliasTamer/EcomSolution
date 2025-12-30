using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class CreateProductCategoryDTO
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }    
    }
}
