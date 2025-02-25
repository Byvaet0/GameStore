using OrderService.Dtos;
using OrderService.Entities;

namespace OrderService.Mapping
{
    public static class OrderItemMappings
    {
        public static OrderItem ToEntity(this OrderItemDto dto, int orderId)
        {
            return new OrderItem
            {
                GameId = dto.GameId,
                GameName = dto.GameName,
                Price = dto.Price, // Цена, если она есть
                OrderId = orderId // Устанавливаем OrderId
            };
        }

        public static OrderItemDto ToDto(this OrderItem entity)
        {
            return new OrderItemDto
            {
                GameId = entity.GameId,
                GameName = entity.GameName,
                Price = entity.Price // Цена
            };
        }

        public static List<OrderItem> ToEntities(this IEnumerable<OrderItemDto> dtos, int orderId)
        {
            return dtos.Select(dto => dto.ToEntity(orderId)).ToList();
        }

        public static List<OrderItemDto> ToDtos(this IEnumerable<OrderItem> entities)
        {
            return entities.Select(entity => entity.ToDto()).ToList();
        }
    }
}
