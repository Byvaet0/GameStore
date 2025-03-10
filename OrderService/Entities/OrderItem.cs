namespace OrderService.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }  
        public int GameId { get; set; }   
        public string GameName { get; set; } = string.Empty;
        public decimal Price { get; set; }  
        
        public Order Order { get; set; }
    }
}
