using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using System.Data;

namespace EcomAPI.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IDbConnection _db;
        public ProductsService(IDbConnection db) {
            _db = db;
        }

        public async Task<ServiceResult<List<Product>>> GetProducts(PaginationParams pagination)
        {
            var allowedColumns = new HashSet<String> { "Id", "Name", "Description", "Price", "StockQuantity", "CreatedAt", "UpdatedAt" };
            var column = allowedColumns.Contains(pagination.SortBy) ? pagination.SortBy : "Id";
            var direction = pagination.SortOrder.Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            var sql = $@"SELECT * 
                        From Products
                        ORDER BY {column} {direction}
                        OFFSET @Skip ROWS
                        FETCH NEXT @Take ROWS ONLY;";

            var results = await _db.QueryAsync<Product>(sql, new { Skip = (pagination.Page - 1) * pagination.PageSize , Take = pagination.PageSize });
            return ServiceResult<List<Product>>.Ok(results.ToList());
        }
    }
}
