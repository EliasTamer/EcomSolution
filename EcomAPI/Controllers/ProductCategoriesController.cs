using EcomAPI.DTOs;
using EcomAPI.Interfaces;
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

        [HttpPost("CreateProductCategory")]
        public async Task<IActionResult> CreateProductCategory(CreateProductCategoryDTO category)
        {

        }
    }
}
