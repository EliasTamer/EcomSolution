using AutoMapper;
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
        private readonly IMapper _mapper;
        public ProductCategoriesService(IDbConnection db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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

        public async Task<bool> EditProductCategory(int id, PatchProductCategoryDTO updatedCategory)
        {
            var categoryToUpdate = await GetProductCategoryDetails(id);
            if(categoryToUpdate == null)
            {
                return false;
            }

            _mapper.Map(updatedCategory, categoryToUpdate);
            categoryToUpdate.UpdatedAt = DateTime.Now;

            var sql = @"UPDATE ProductCategories
                      SET Title = @Title, Description = @Description, ImageUrl = @ImageUrl, UpdatedAt = @UpdatedAt
                      WHERE Id = @Id";

            var rowsAffected = await _db.ExecuteAsync(sql, categoryToUpdate);
            return rowsAffected > 0;
            }
        } 
    }

