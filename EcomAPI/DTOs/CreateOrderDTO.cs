using System.ComponentModel.DataAnnotations;

namespace EcomAPI.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        [Required]
        public List<OrderItemDTO> OrderItems {get; set;} = [];
     }
}
