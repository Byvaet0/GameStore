namespace CartService.DTOs
{
    public class CartDto
    {
        public int CartId { get; set; }  // Добавлено CartId
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
