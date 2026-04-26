using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IProductsService
    {
        Task<ServiceResult<List<Product>>> GetProducts(ProductListingFilters filters);
    }
}
