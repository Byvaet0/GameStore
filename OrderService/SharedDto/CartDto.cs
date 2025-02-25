namespace OrderService.SharedDto
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemDto
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public decimal Price { get; set; }
    }
}
