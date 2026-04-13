using AutoMapper;
using Dapper;
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
        public ProductCategoriesService(IDbConnection db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<ProductCategory>>> GetProductCategories(PaginationParams pagination)
        {
            try
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
            catch (Exception ex) {
                return ServiceResult<List<ProductCategory>>.Fail("Database operation failed");
            }
        }

        public async Task<ServiceResult<int>> CreateProductCategory(CreateProductCategoryDTO category)
        {
            ProductCategory productCategory = _mapper.Map<ProductCategory>(category);

            try
            {
                var sql = @"INSERT INTO ProductCategories(Title, Description, ImageUrl)
                          VALUES(@Title, @Description, @ImageUrl);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

                var productCategoryId = await _db.QuerySingleAsync<int>(sql, productCategory);
                return ServiceResult<int>.Ok(productCategoryId);

            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail("Category creation failed");
            }
        }

        public async Task<ServiceResult<bool>> DeleteProductCategory(int categoryId)
        {
            try
            {
                var sql = @"DELETE FROM ProductCategories WHERE Id = @categoryId";
                var affectedRows = await _db.ExecuteAsync(sql, new { categoryId });

                if (affectedRows == 1)
                {
                    return ServiceResult<bool>.Ok(true);
                }
                else
                {
                    return ServiceResult<bool>.Fail("Product not found");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Database operation failed");
            }

        }

        public async Task<ServiceResult<ProductCategory>> GetProductCategoryDetails(int categoryId)
        {
            try
            {
                var sql = @"SELECT Id, Title, Description, ImageUrl, CreatedAt, UpdatedAt
                        FROM ProductCategories WHERE Id = @categoryId";

                var result = await _db.QueryFirstOrDefaultAsync<ProductCategory?>(sql, new { categoryId });
                if (result == null)
                {
                    return ServiceResult<ProductCategory>.Fail("Category not found");
                }
                else
                {
                    return ServiceResult<ProductCategory>.Ok(result);
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<ProductCategory>.Fail("Database operation failed");
            }
        }

        public async Task<ServiceResult<bool>> EditProductCategory(int id, PatchProductCategoryDTO updatedCategory)
        {
            try
            {
                var sql = @"UPDATE ProductCategories
                SET Title = COALESCE(@Title, Title),
                    Description = COALESCE(@Description, Description),
                    ImageUrl = COALESCE(@ImageUrl, ImageUrl),
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

                var parameters = new
                {
                    Id = id,
                    updatedCategory.Title,
                    updatedCategory.Description,
                    updatedCategory.ImageUrl,
                    UpdatedAt = DateTime.UtcNow
                };

                var rowsAffected = await _db.ExecuteAsync(sql, parameters);
                return rowsAffected > 0
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.Fail("Category not found or update failed");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail("Database operation failed");
            }
        }

    }
}