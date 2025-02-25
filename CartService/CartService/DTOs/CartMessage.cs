namespace CartService.DTOs
{
    public class CartMessage
    {
        public int UserId { get; set; }  
        public int GameId { get; set; }  
        public string GameName { get; set; }  
        public decimal Price { get; set; }  
    }
}
