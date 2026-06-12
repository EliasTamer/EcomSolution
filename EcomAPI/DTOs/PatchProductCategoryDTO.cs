namespace EcomAPI.DTOs
{
    public class PatchProductCategoryDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}