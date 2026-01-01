using Dapper;
using EcomAPI.DTOs;
using EcomAPI.Entities;
using EcomAPI.Interfaces;
using System.Data;

namespace EcomAPI.Services
{
    public class ProductCategoriesService : IProductCategoriesService
    {
        private readonly IDbConnection _db;
        public ProductCategoriesService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> CreateProductCategory(CreateProductCategoryDTO category)
        {
            ProductCategory productCategory = new ProductCategory()
            {
                Title = category.Title,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
            };

            var sql = @"INSERT INTO ProductCategories(Title, Description, ImageUrl)
                      VALUES(@Title, @Description, @ImageUrl)
                      SELECT CAST(SCOPE_IDENTITY() as int)";

            return await _db.QuerySingleAsync<int>(sql, productCategory);

        }

        public async Task<int> DeleteProductCategory(int categoryId)
        {
            var sql = @"DELETE FROM ProductCategories WHERE Id = @categoryId";
            return await _db.ExecuteAsync(sql, new { categoryId = categoryId });
        }

        public async Task<ProductCategory?> GetProductCategoryDetails(int categoryId)
        {
            var sql = @"SELECT * FROM ProductCategories WHERE Id = @categoryId";
            return await _db.QueryFirstOrDefaultAsync<ProductCategory?>(sql, new { categoryId = categoryId });
        }

        public async Task<bool?> EditProductCategory(int id, PatchProductCategoryDTO updatedCategory)
        {
            var categoryToUpdate = await GetProductCategoryDetails(id);
            if(categoryToUpdate == null)
            {
                return false;
            }
            else
            {
                var updates = new List<string>();
                var parameters = new DynamicParameters();

                parameters.Add("Id", id);

                if(updatedCategory.Title != null)
                {
                    updates.Add("Title = @Title");
                    parameters.Add("Title", updatedCategory.Title);
                }

                if (updatedCategory.Description != null)
                {
                    updates.Add("Description  = @Description ");
                    parameters.Add("Description ", updatedCategory.Description);
                }

                if (updatedCategory.ImageUrl != null)
                {
                    updates.Add("ImageUrl  = @ImageUrl ");
                    parameters.Add("ImageUrl ", updatedCategory.ImageUrl);
                }

                updates.Add("UpdatedAt = @UpdatedAt");
                parameters.Add("UpdatedAt", DateTime.UtcNow);

                if (updates.Count == 1) return false;

                var sql = $"UPDATE ProductCategories SET {string.Join(", ", updates)} WHERE Id = @Id";
                var rowsAffected = await _db.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
        } 
    }
}
