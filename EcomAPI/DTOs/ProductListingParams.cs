namespace EcomAPI.DTOs
{
    public class ProductListingFilters : PaginationParams
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int? CategoryId {  get; set; }
        public bool? IsAvailable { get; set; }
        public string? Search {  get; set; }
    }
}   
