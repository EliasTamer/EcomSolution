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

        public ProductsController(IProductsService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [HttpGet("ProductListing")]
        public async Task<IActionResult> GetProductListing([FromQuery] ProductListingFilters filters)
        {
            ApiResponse response = new();

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

            if (result.Success)
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
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO product)
        {
            ApiResponse response = new();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(response);
            }

            var result = await _productService.CreateProduct(product);

            if (result.Data > 0)
            {
                response.Success = true;
                response.Status = 200;
                response.Data = result.Data;
                return Ok(response);
            }

            response.Message = "An error has occured, please try again.";
            response.Status = 500;
            return BadRequest(response);
        }

        [Authorize]
        [HttpDelete("DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct([FromQuery] int productId)
        {
            ApiResponse response = new();

            var result = await _productService.DeleteProduct(productId);

            if (result.Success)
            {
                response.Success = true;
                response.Status = 200;
                response.Data = result.Data;
                return Ok(response);
            }

            response.Status = 404;
            response.Message = result.Message;
            return BadRequest(response);
        }

        [Authorize]
        [HttpPatch("PatchProduct/{productId}")]
        public async Task<IActionResult> PatchProduct([FromQuery] int productId, [FromBody] PatchProductDTO product)
        {
            ApiResponse response = new();

            if (!ModelState.IsValid)
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(response);
            }

            var result = await _productService.PatchProduct(productId, product);

            if (result.Success) {
                response.Success = true;
                response.Status = 200;
                response.Data = result.Data;
                response.Message = "Product patched";

                return Ok(response);
            }

            response.Message = result.Message;
            response.Status = 400;
            return BadRequest(response);
        }
    }
}
