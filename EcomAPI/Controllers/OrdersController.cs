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
        private readonly OrdersService _ordersService;
        public OrdersController(OrdersService ordersService)
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
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = ModelState.Values.SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();
                return BadRequest(response);
            }


            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                response.Status = 403;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            var result = await _ordersService.PlaceOrder(userId, order);

            if (result.Success) { 
                response.Status = 200;
                response.Success = true;
                response.Message = "Order placed";
                return Ok(response);
            }

            response.Status = 400;
            response.Message = "An error has occured while placing order, please try again.";
            return BadRequest(response);
        }

    }
}