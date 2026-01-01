using EcomAPI.DTOs;
using EcomAPI.Entities;

namespace EcomAPI.Interfaces
{
    public interface IProductCategoriesService
    {
        public Task<int> CreateProductCategory(CreateProductCategoryDTO category); 
        public Task<int> DeleteProductCategory (int id);
        public Task<ProductCategory?> GetProductCategoryDetails(int id);
        public Task<bool> EditProductCategory(int id, PatchProductCategoryDTO category);
    }
}
