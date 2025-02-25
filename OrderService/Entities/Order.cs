namespace OrderService.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }  
        public string Status { get; set; }
        public int TotalQuantity { get; set; }  
        public List<OrderItem> OrderItems { get; set; } = new();
        
    }
}
