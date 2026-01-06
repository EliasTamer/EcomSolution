using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        [HttpPost("CreateProductCategory")]
        public async Task<IActionResult> CreateProductCategory([FromBody] CreateProductCategoryDTO category)
        {
            ApiResponse response = new ApiResponse();
            
            if(!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();
                return BadRequest(response);
            }

            try
            {
                var createdId = await _productCategoriesService.CreateProductCategory(category);

                response.Status = 200;
                response.Success = true;
                response.Message = "Product category created successfuly.";
                response.Data = new { productCategoryId =  createdId };
                return Ok(response);
            }
            catch (Exception ex) { 
                response.Status =500;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }

        [Authorize]
        [HttpDelete("DeleteProductCategory/{categoryId}")]
        public async Task<IActionResult> DeleteProductCategory([FromRoute] int categoryId)
        {
            ApiResponse response = new ApiResponse();

            try
            {
                var affectedRows = await _productCategoriesService.DeleteProductCategory(categoryId);

                if(affectedRows > 0)
                {
                    response.Success = true;
                    response.Status = 200;
                    response.Message = "Product category was deleted successfully.";
                    return Ok(response);
                } else
                {
                    response.Status = 404;
                    response.Message = "Product doesn't exist";
                    return NotFound(response);
                }
            }
            catch (Exception ex) {
                response.Status = 500;
                response.Message = ex.Message;
                return StatusCode(500, response);
            }
        }
        [Authorize]
        [HttpGet("GetProductCategoryDetails/{categoryId}")]
        public async Task<IActionResult> GetProductCategoryDetails([FromRoute] int categoryId)
        {
            ApiResponse response = new ApiResponse();

            var categoryDetails = await _productCategoriesService.GetProductCategoryDetails(categoryId);

            if(categoryDetails == null)
            {
                response.Status = 404;
                response.Message = "Product category doesn't exist";
                return NotFound(response);
            }
            else
            {
                response.Status = 200;
                response.Data = categoryDetails;
                response.Success = true;
                return Ok(response);
            }
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

            bool isEdited = await _productCategoriesService.EditProductCategory(categoryId, updatedFields);

            if(!isEdited)
            {
                response.Status = 400;
                response.Message = "Editing category has failed.";
                return BadRequest(response);
            }
            else
            {
                response.Success = true;
                response.Status = 200;
                response.Message = "Category was edited successfuly.";
                return Ok(response);
            }
        }
    }
}
