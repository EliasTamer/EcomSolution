using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using EcomAPI.Utils;
using System.Data;

namespace EcomAPI.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IDbConnection _db;
        private readonly IFileService _fileService;
        public ProductsService(IDbConnection db, [FromKeyedServices(FileStores.ProductPhotos)] IFileService fileService)
        {
            _db = db;
            _fileService = fileService;
        }

        public async Task<ServiceResult<List<Product>>> GetProducts(ProductListingFilters filters)
        {
            var allowedColumns = new HashSet<string> { "Id", "Name", "Description", "Price", "StockQuantity", "CreatedAt", "UpdatedAt" };
            var column = allowedColumns.Contains(filters.SortBy) ? filters.SortBy : "Id";
            var direction = filters.SortOrder.Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (filters.MinPrice != null)
            {
                conditions.Add("Price >= @MinPrice");
                parameters.Add("MinPrice", filters.MinPrice);
            }

            if (filters.MaxPrice != null)
            {
                conditions.Add("Price <= @MaxPrice");
                parameters.Add("MaxPrice", filters.MaxPrice);
            }

            if (filters.CategoryId != null)
            {
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
            string? imagePath = null;

            if (product.ImageUrl != null)
            {
                var storeImageResult = await _fileService.StoreFile(product.ImageUrl);

                if (!storeImageResult.Success)
                {
                    return ServiceResult<int>.Fail(storeImageResult.Message);
                }
                imagePath = storeImageResult.Data;
            }

            var sql = @"INSERT INTO Products(Name, Description, Price, CategoryId, ImageUrl, StockQuantity, IsAvailable)
                        VALUES(@Name, @Description, @Price, @CategoryId, @ImageUrl, @StockQuantity, @IsAvailable) 
                        SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new DynamicParameters();
            parameters.Add("Name", product.Name);
            parameters.Add("Description", product.Description);
            parameters.Add("Price", product.Price);
            parameters.Add("CategoryId", product.CategoryId);
            parameters.Add("ImageUrl", imagePath);
            parameters.Add("StockQuantity", product.StockQuantity);
            parameters.Add("IsAvailable", product.StockQuantity > 0);

            int result = await _db.ExecuteAsync(sql, parameters);
            return ServiceResult<int>.Ok(result);

        }

        public async Task<ServiceResult<bool>> DeleteProduct(int Id)
        {
            var sql = @"DELETE FROM Product 
                        WHERE Id = @Id";

            var affectedRows = await _db.ExecuteAsync(sql, new { Id });
            return affectedRows > 0 ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.Fail("Product not found");
        }

        public async Task<ServiceResult<bool>> PatchProduct(int id, PatchProductDTO product)
        {
            string? newImagePath = null;

            if (product.ImageUrl != null)
            {
                var storeImageResult = await _fileService.StoreFile(product.ImageUrl);

                if (!storeImageResult.Success)
                {
                    return ServiceResult<bool>.Fail(storeImageResult.Message);
                }
                newImagePath = storeImageResult.Data;
            }

            var sql = $"""
                UPDATE Products
                    SET 
                        Name = COALESCE(@FirstName, FirstName),
                        Description = COALESCE(@Description, Description),
                        Price = COALESCE(@Price, Price),
                        CategoryId = COALESCE(@CategoryId, CategoryId),
                        ImageUrl = COALESCE(@ImageUrl, ImageUrl),
                        StockQuantity = COALESCE(@StockQuantity, StockQuantity),
                        IsAvailable = COALESCE(@IsAvailable, IsAvailable)
                OUTPUT deleted.Id as Id,  deleted.ImageUrl as OldImage 
                WHERE Id = @Id
                """;

            var row = await _db.QuerySingleOrDefaultAsync<(int Id, string? OldImage)>(sql, new
            {
                Id = id,
                product.Description,
                product.Price,
                product.CategoryId,
                product.ImageUrl,
                product.StockQuantity,
                product.IsAvailable,
            });

            if (row.Id == 0)
            {
                if (newImagePath != null)
                {
                    _fileService.DeleteFile(newImagePath);
                }
                return ServiceResult<bool>.Fail("User not found");
            }

            if (newImagePath != null && !string.IsNullOrEmpty(row.OldImage))
            {
                _fileService.DeleteFile(row.OldImage);
            }

            return ServiceResult<bool>.Ok(true);
        }

    }
}
