using EcomAPI.DTOs;

namespace EcomAPI.Interfaces
{
    public interface IProductCategoriesService
    {
        public Task<int> CreateProductCategory(CreateProductCategoryDTO category); 
        public Task<int> DeleteProductCategory (int id);
    }
}
