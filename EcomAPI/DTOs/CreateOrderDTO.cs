namespace EcomAPI.DTOs
{
    public class CreateOrderDTO
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public List<OrderItemDTO> OrderItems {get; set;} = [];
     }
}
