using Dapper;
using AutoMapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using System.Data;

namespace EcomAPI.Services
{
    public class ProductCategoriesService : IProductCategoriesService
    {
        private readonly IDbConnection _db;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductCategoriesService(IDbConnection db, IMapper mapper, [FromKeyedServices("productCategoryPhotos")] IFileService fileService)
        {
            _db = db;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<ServiceResult<List<ProductCategory>>> GetProductCategories(PaginationParams pagination)
        {
            var allowedColumns = new HashSet<string> { "Id", "Title", "CreatedAt", "UpdatedAt" };
            var column = allowedColumns.Contains(pagination.SortBy) ? pagination.SortBy : "Id";
            var direction = pagination.SortOrder.Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

            var sql = $@"SELECT Title, Description, ImageUrl, CreatedAt, UpdatedAt
                        FROM ProductCategories
                        ORDER BY {column} {direction}
                        OFFSET @Skip ROWS
                        FETCH NEXT @Take ROWS ONLY;";

            var results = await _db.QueryAsync<ProductCategory>(sql, new
            {
                Skip = (pagination.Page - 1) * pagination.PageSize,
                Take = pagination.PageSize
            });

            return ServiceResult<List<ProductCategory>>.Ok(results.ToList());
        }

        public async Task<ServiceResult<int>> CreateProductCategory(CreateProductCategoryDTO category)
        {
            var productCategoryPhoto = category.ImageUrl;
            var imagePath = string.Empty;

            if (productCategoryPhoto != null)
            {
                var storeImageResult = await _fileService.StoreFile(productCategoryPhoto);
                if (storeImageResult.Success)
                {
                    imagePath = storeImageResult.Data;
                }
                else
                {
                    return ServiceResult<int>.Fail(storeImageResult.Message);
                }
            }

            var sql = @"INSERT INTO ProductCategories(Title, Description, ImageUrl)
                      VALUES(@Title, @Description, @ImageUrl);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

            var productCategoryId = await _db.QuerySingleAsync<int>(sql, new
            {
                Title = category.Title,
                Description = category.Description,
                ImageUrl = imagePath,
            });
            return ServiceResult<int>.Ok(productCategoryId);
        }

        public async Task<ServiceResult<bool>> DeleteProductCategory(int categoryId)
        {
            var sql = @"DELETE FROM ProductCategories WHERE Id = @categoryId";
            var affectedRows = await _db.ExecuteAsync(sql, new { categoryId });

            return affectedRows == 1
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Product not found");
        }

        public async Task<ServiceResult<ProductCategory>> GetProductCategoryDetails(int categoryId)
        {
            var sql = @"SELECT Id, Title, Description, ImageUrl, CreatedAt, UpdatedAt
                    FROM ProductCategories WHERE Id = @categoryId";

            var result = await _db.QueryFirstOrDefaultAsync<ProductCategory?>(sql, new { categoryId });

            return result != null
                ? ServiceResult<ProductCategory>.Ok(result)
                : ServiceResult<ProductCategory>.Fail("Category not found");
        }

        public async Task<ServiceResult<bool>> PatchProductCategory(int id, PatchProductCategoryDTO category)
        {
            var productCategoryPhoto = category.ImageUrl;
            var imagePath = string.Empty;

            if (productCategoryPhoto != null)
            {
                var storeImageResult = await _fileService.StoreFile(productCategoryPhoto);
                if (storeImageResult.Success)
                {
                    imagePath = storeImageResult.Data;
                }
                else
                {
                    return ServiceResult<bool>.Fail(storeImageResult.Message);
                }
            }

            var sql = @"UPDATE ProductCategories
                        SET Title = COALESCE(@Title, Title),
                            Description = COALESCE(@Description, Description),
                            ImageUrl = COALESCE(@ImageUrl, ImageUrl),
                            UpdatedAt = @UpdatedAt
                        WHERE Id = @Id";

            var parameters = new
            {
                Id = id,
                category.Title,
                category.Description,
                ImageUrl = imagePath,
                UpdatedAt = DateTime.UtcNow
            };

            var rowsAffected = await _db.ExecuteAsync(sql, parameters);
            return rowsAffected > 0
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Category not found or update failed");
        }
    }
}