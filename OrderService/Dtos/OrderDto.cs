using OrderService.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }  
    public decimal TotalPrice { get; set; }  // Добавлено поле для общей суммы
    public int Quantity { get; set; }        // Добавлено поле для количества товаров
    public List<OrderItemDto> OrderItems { get; set; } = new(); // Список элементов заказа
}
