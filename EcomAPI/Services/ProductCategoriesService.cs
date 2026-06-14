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
            string? newImagePath = null;

            if (category.ImageUrl != null)
            {
                var storeImageResult = await _fileService.StoreFile(category.ImageUrl);

                if (!storeImageResult.Success)
                {
                    return ServiceResult<int>.Fail(storeImageResult.Message);
                }

                newImagePath = storeImageResult.Data;
            }

            var sql = @"INSERT INTO ProductCategories(Title, Description, ImageUrl)
                      VALUES(@Title, @Description, @ImageUrl);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

            var productCategoryId = await _db.QuerySingleAsync<int>(sql, new
            {
                Title = category.Title,
                Description = category.Description,
                ImageUrl = newImagePath,
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
            string? newImagePath = null;

            if (category.ImageUrl != null)
            {
                var storeImageResult = await _fileService.StoreFile(category.ImageUrl);

                if (!storeImageResult.Success)
                {
                    return ServiceResult<bool>.Fail(storeImageResult.Message);
                }

                newImagePath = storeImageResult.Data;
            }

            var sql = @"UPDATE ProductCategories
                        SET Title = COALESCE(@Title, Title),
                            Description = COALESCE(@Description, Description),
                            ImageUrl = COALESCE(@ImageUrl, ImageUrl),
                            UpdatedAt = @UpdatedAt
                            OUTPUT deleted.Id as Id, deleted.ImageUrl as OldPhoto
                        WHERE Id = @Id";

            var parameters = new
            {
                Id = id,
                category.Title,
                category.Description,
                ImageUrl = newImagePath,
                UpdatedAt = DateTime.UtcNow
            };

            var row = await _db.QuerySingleOrDefaultAsync<(int Id, string? OldPhoto)>(sql, parameters);

            if (row.Id == 0)
            {
                if (newImagePath != null)
                {
                    _fileService.DeleteFile(newImagePath);
                }
                return ServiceResult<bool>.Fail("User not found");
            }

            if (newImagePath != null && !string.IsNullOrEmpty(row.OldPhoto))
            {
                _fileService.DeleteFile(row.OldPhoto);
            }

            return ServiceResult<bool>.Ok(true);
        }
    }
}