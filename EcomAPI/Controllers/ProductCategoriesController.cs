using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomAPI.Controllers
{
    [ApiController]
    [Route("api/ProductCategories")]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoriesService _productCategoriesService;

        public ProductCategoriesController(IProductCategoriesService productCategoriesService)
        {
            _productCategoriesService = productCategoriesService;
        }

        [Authorize]
        [HttpGet("ProductCategoriesListing")]
        public async Task<IActionResult> GetProductCategoriesListing([FromQuery] PaginationParams pagination)
        {
            ApiResponse response = new ApiResponse();

            var getCategoriesResult = await _productCategoriesService.GetProductCategories(pagination);

            if (getCategoriesResult.Success)
            {
                response.Status = 200;
                response.Success = true;
                response.Data = getCategoriesResult.Data;
                return Ok(response);
            }

            response.Status = 400;
            response.Message = getCategoriesResult.Message;
            return BadRequest(response);
        }

        [Authorize]
        [HttpPost("CreateProductCategory")]
        public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryDTO category)
        {
            ApiResponse response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();
                return BadRequest(response);
            }

            var createCategoryResult = await _productCategoriesService.CreateProductCategory(category);

            if (createCategoryResult.Success)
            {
                response.Status = 200;
                response.Success = true;
                response.Message = "Product category created successfuly.";
                response.Data = new { productCategoryId = createCategoryResult.Data };
                return Ok(response);
            }

            response.Status = 400;
            response.Message = "Error occured when creating product category";
            return BadRequest(response);
        }

        [Authorize]
        [HttpDelete("DeleteProductCategory/{categoryId}")]
        public async Task<IActionResult> DeleteProductCategory([FromRoute] int categoryId)
        {
            ApiResponse response = new ApiResponse();

            var deleteCategoryResult = await _productCategoriesService.DeleteProductCategory(categoryId);

            if (deleteCategoryResult.Success)
            {
                response.Success = true;
                response.Status = 200;
                response.Message = "Product category was deleted successfully.";
                return Ok(response);
            }

            response.Status = 400;
            response.Message = "Product deletion failed.";
            return BadRequest(response);
        }

        [Authorize]
        [HttpGet("GetProductCategoryDetails/{categoryId}")]
        public async Task<IActionResult> GetProductCategoryDetails([FromRoute] int categoryId)
        {
            ApiResponse response = new ApiResponse();

            var getCategoryDetailsResult = await _productCategoriesService.GetProductCategoryDetails(categoryId);

            if (!getCategoryDetailsResult.Success)
            {
                response.Status = 404;
                response.Message = "Product category doesn't exist";
                return NotFound(response);
            }

            response.Status = 200;
            response.Data = getCategoryDetailsResult.Data;
            response.Success = true;
            return Ok(response);
        }

        [Authorize]
        [HttpPatch("EditProductCategory/{categoryId}")]
        public async Task<IActionResult> EditProductCategory([FromRoute] int categoryId, [FromBody] PatchProductCategoryDTO updatedFields)
        {
            ApiResponse response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();
                return BadRequest(response);
            }

            var editCategoryResult = await _productCategoriesService.EditProductCategory(categoryId, updatedFields);

            if (!editCategoryResult.Success)
            {
                response.Status = 400;
                response.Message = "Editing category has failed.";
                return BadRequest(response);
            }

            response.Success = true;
            response.Status = 200;
            response.Message = "Category was edited successfuly.";
            return Ok(response);
        }
    }
}