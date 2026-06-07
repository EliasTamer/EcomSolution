using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IProductsService
    {
        Task<ServiceResult<List<Product>>> GetProducts(ProductListingFilters filters);
        Task<ServiceResult<int>> CreateProduct(CreateProductDTO product);
        Task<ServiceResult<bool>> DeleteProduct(int id);
    }
}
