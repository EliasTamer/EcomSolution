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

        public OrdersService(IDbConnection db)
        {
            _db = db;
        }

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

            var total = order.OrderItems.Sum(i => products[i.ProductId].Price * i.Quantity);
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            if(_db.State != ConnectionState.Open) _db.Open();
            using var tx = _db.BeginTransaction();

            var orderId = await _db.ExecuteScalarAsync<int>("""
                INSERT INTO ORDERS(UserId, OrderNumber, TotalAmount, ShippingAddress, PaymentMethod)
                OUTPUT INSERTED.Id
                VALUES (@UserId, @OrderNumber, @TotalAmount, @ShippingAddress, @PaymentMethod)
                """,
                new { UserId = userId, OrderNumber = orderNumber, TotalAmount = total, order.ShippingAddress, order.PaymentMethod}, tx);

            foreach (var item in order.OrderItems) { 
            
                var price = products[item.ProductId].Price;

                await _db.ExecuteAsync("""
                    INSERT INTO OrderItems(OrderId, ProductId, Quantity, UnitPrice, Subtotal)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @Subtotal)
                    """, new { OrderId = orderId,item.ProductId, item.Quantity, UnitPrice = price, Subtotal = price * item.Quantity }, tx);

                var affected = await _db.ExecuteAsync("""
                    UPDATE Products SET StockQuantity = StockQuantity - @Quantity
                    WHERE Id = @ProductId AND StockQuantity >= @Quantity
                    """, new { item.Quantity, item.ProductId}, tx);

                if(affected == 0)
                {
                    tx.Rollback();
                    return ServiceResult<string>.Fail($"Insufficient stock for product {item.ProductId}");
                }
            }

            tx.Commit();
            return ServiceResult<string>.Ok(orderNumber);
        }
    }
}
