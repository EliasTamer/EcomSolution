using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IProductCategoriesService
    {
        public Task<ServiceResult<List<ProductCategory>>> GetProductCategories(PaginationParams pagination);
        public Task<ServiceResult<int>> CreateProductCategory(CreateProductCategoryDTO category);
        public Task<ServiceResult<bool>> DeleteProductCategory(int id);
        public Task<ServiceResult<ProductCategory>> GetProductCategoryDetails(int id);
        public Task<ServiceResult<bool>> PatchProductCategory(int id, PatchProductCategoryDTO category);
    }
}
