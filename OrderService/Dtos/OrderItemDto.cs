namespace OrderService.Dtos
{
    public class OrderItemDto
    {
        public int GameId { get; set; }
        public string GameName { get; set; }
        public decimal Price { get; set; }
    }
}
