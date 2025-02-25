using OrderService.Entities;
using OrderService.Dtos;

namespace OrderService.Mapping
{
    public static class OrderMapping
    {
        // Преобразование сущности Order в DTO OrderDto
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                TotalPrice = order.TotalPrice,  // Добавляем поле TotalPrice в DTO
                Quantity = order.OrderItems.Count, // Количество товаров - считаем по элементам в заказе
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    GameId = oi.GameId,
                    GameName = oi.GameName,
                    Price = oi.Price
                }).ToList()
            };
        }

        // Преобразование DTO OrderDto в сущность Order
        public static Order ToEntity(this OrderDto orderDto)
        {
            return new Order
            {
                Id = orderDto.Id,
                UserId = orderDto.UserId,
                CreatedAt = orderDto.CreatedAt,
                Status = orderDto.Status,
                TotalPrice = orderDto.TotalPrice,  // Добавляем поле TotalPrice из DTO
                OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
                {
                    GameId = oi.GameId,
                    GameName = oi.GameName,
                    Price = oi.Price
                }).ToList()
            };
        }
    }
}
