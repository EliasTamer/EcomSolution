namespace EcomAPI.DTOs
{
    public class PatchProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public double? Price { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
