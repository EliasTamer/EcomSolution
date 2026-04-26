using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace EcomAPI.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IDbConnection _db;
        public ProductsService(IDbConnection db) {
            _db = db;
        }

        public async Task<ServiceResult<List<Product>>> GetProducts(ProductListingFilters filters)
        {
            var allowedColumns = new HashSet<string> { "Id", "Name", "Description", "Price", "StockQuantity", "CreatedAt", "UpdatedAt" };
            var column = allowedColumns.Contains(filters.SortBy) ? filters.SortBy : "Id";
            var direction = filters.SortOrder.Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if(filters.MinPrice != null)
            {
                conditions.Add("Price >= @MinPrice");
                parameters.Add("MinPrice", filters.MinPrice);
            }

            if (filters.MaxPrice != null) {
                conditions.Add("Price <= @MaxPrice");
                parameters.Add("MaxPrice", filters.MaxPrice);
            }

            if (filters.CategoryId != null) {
                conditions.Add("CategoryId = @CategoryId");
                parameters.Add("CategoryId", filters.CategoryId);
            }

            if (filters.IsAvailable != null)
            {
                conditions.Add("IsAvailable = @IsAvailable");
                parameters.Add("IsAvailable", filters.IsAvailable);
            }

            if (!string.IsNullOrEmpty(filters.Search))
            {
                conditions.Add("Name LIKE @Search");
                parameters.Add("Search", $"%{filters.Search}%");
            }

            var whereClause = conditions.Count > 0 ? "WHERE" + string.Join(" AND ", conditions) : "";


            var sql = $@"SELECT Id, Name, Description, Price, CategoryId, ImageUrl, StockQuantity, IsAvailable, CreatedAt, UpdatedAt
                        From Products
                        {whereClause}
                        ORDER BY {column} {direction}
                        OFFSET @Skip ROWS
                        FETCH NEXT @Take ROWS ONLY;";

            parameters.Add("Skip", (filters.Page - 1) * filters.PageSize);
            parameters.Add("Take", filters.PageSize);

            var results = await _db.QueryAsync<Product>(sql, parameters);
            return ServiceResult<List<Product>>.Ok(results.ToList());
        }

        public async Task<ServiceResult<int>> CreateProduct(CreateProductDTO product)
        {
            var sql = @"INSERT INTO Products(Name, Description, Price, CategoryId, ImageUrl, StockQuantity, IsAvailable)
                        VALUES(@Name, @Description, @Price, @CategoryId, @ImageUrl, @StockQuantity, @IsAvailable) 
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new DynamicParameters();
            parameters.Add("Name", product.Name);
            parameters.Add("Description", product.Description);
            parameters.Add("Price", product.Price);
            parameters.Add("CategoryId", product.CategoryId);
            parameters.Add("ImageUrl", product.ImageUrl);
            parameters.Add("StockQuantity", product.StockQuantity);
            parameters.Add("IsAvailable", product.StockQuantity > 0);

            int result = await _db.ExecuteAsync(sql, parameters);
            return ServiceResult<int>.Ok(result);

        }

        public async Task<ServiceResult<bool>> DeleteProduct(int Id)
        {
            var sql = @"DELETE FROM Product 
                        WHERE Id = @Id";

            var affectedRows = await _db.ExecuteAsync(sql, new { Id});
            return affectedRows > 0 ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.Fail("Product not found");
        }

    }
}
