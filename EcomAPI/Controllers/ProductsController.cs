using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomAPI.Controllers
{
    [ApiController]
    [Route("api/Products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productService;

        public ProductsController(IProductsService productService) {
            _productService = productService;
        }

        [Authorize]
        [HttpGet("ProductListing")]
        public async Task<IActionResult> GetProductListing([FromQuery] ProductListingFilters filters)
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

            var result = await _productService.GetProducts(filters);

            if(result.Success)
            {
                response.Status = 200;
                response.Success = true;
                response.Data = result.Data;
                return Ok(response);
            }

            response.Status = 400;
            response.Message = result.Message;
            return BadRequest(response);
        }
        [Authorize]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO)
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


        }
    }
}
