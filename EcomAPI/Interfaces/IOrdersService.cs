using EcomAPI.DTOs;
using EcomAPI.Responses;

namespace EcomAPI.Interfaces
{
    public interface IOrdersService
    {
        Task<ServiceResult<string>> PlaceOrder(int userId, CreateOrderDTO order);
    }
}
