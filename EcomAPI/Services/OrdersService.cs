using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Responses;
using System.Data;

namespace EcomAPI.Services
{
    public class OrdersService
    {
        private readonly IDbConnection _db;

        public async Task<ServiceResult<string>> PlaceOrder (int userId, CreateOrderDTO order)
        {
            var productIds = order.OrderItems.Select(i => i.ProductId).ToList();

            var getProductsQuery = "SELECT Id, Price, StockQuantity, IsAvailable FROM Products WHERE Id IN @Ids";

            var products = (await _db.QueryAsync<Product>(getProductsQuery, new { Ids = productIds})).ToDictionary(p => p.Id);


            foreach (var item in order.OrderItems)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    return ServiceResult<string>.Fail($"Product {item.ProductId} not found");

                if (!product.IsAvailable)
                    return ServiceResult<string>.Fail($"Product {item.ProductId} not available");

                if (product.StockQuantity < item.Quantity)
                    return ServiceResult<string>.Fail($"Insufficient stock for product {item.ProductId}");
            }
        }
    }
}
