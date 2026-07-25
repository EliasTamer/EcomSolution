using EcomAPI.DTOs;
using EcomAPI.Interfaces;
using EcomAPI.Responses;
using EcomAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcomAPI.Controllers
{
    [ApiController]
    [Route("api/Orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [Authorize]
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(CreateOrderDTO order)
        {
            ApiResponse response = new();

            if (!ModelState.IsValid)
            {
                response.Status = StatusCodes.Status400BadRequest;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();
                return BadRequest(response);
            }


            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                response.Status = StatusCodes.Status403Forbidden;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            var result = await _ordersService.PlaceOrder(userId, order);

            if (result.Success) { 
                response.Status = StatusCodes.Status200OK;
                response.Success = true;
                response.Message = "Order placed";
                return Ok(response);
            }

            response.Status = StatusCodes.Status400BadRequest;
            response.Message = "An error has occured while placing order, please try again.";
            return BadRequest(response);
        }

    }
}