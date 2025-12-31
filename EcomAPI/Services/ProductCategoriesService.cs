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
        public ProductCategoriesService(IDbConnection db) {
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

        public async Task<int> DeleteProductCategory (int categoryId)
        {
            var sql = @"DELETE FROM ProductCategories WHERE Id = @categoryId";
            return await _db.ExecuteAsync(sql, new { categoryId = categoryId });
        }

    }
}
