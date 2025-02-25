namespace CartService.Entities
{
    public class Cart
    {
        public int CartId { get; set; }  // Уникальный идентификатор корзины
        public int UserId { get; set; }  // Идентификатор пользователя
        public List<CartItem> Items { get; set; } = new();  // Связь с элементами корзины
        public int TotalQuantity { get; set; }  // Общее количество товаров в корзине
        public decimal TotalPrice { get; set; }  // Общая цена товаров в корзине
    }
}
