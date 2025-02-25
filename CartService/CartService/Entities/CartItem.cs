namespace CartService.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }  // Уникальный идентификатор для CartItem
        public int CartId { get; set; }  // Внешний ключ для связи с Cart
        public int GameId { get; set; }
        public string GameName { get; set; }
        public decimal Price { get; set; }

        public Cart Cart { get; set; }  // Навигационное свойство для связи с Cart
    }
}
