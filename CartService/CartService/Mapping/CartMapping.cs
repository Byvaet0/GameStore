using CartService.Entities;
using CartService.DTOs;

namespace CartService.Mapping
{
    public static class CartMapping
    {
        public static CartDto ToDto(Cart cart)
        {
            return new CartDto
            {
                CartId = cart.CartId,  // Добавлено CartId
                UserId = cart.UserId,
                Items = cart.Items.Select(i => CartItemMapping.ToDto(i)).ToList(),
                TotalQuantity = cart.TotalQuantity,
                TotalPrice = cart.TotalPrice
            };
        }

        public static Cart ToEntity(CartDto cartDto)
        {
            return new Cart
            {
                CartId = cartDto.CartId,  // Учитываем CartId
                UserId = cartDto.UserId,
                Items = cartDto.Items.Select(i => CartItemMapping.ToEntity(i)).ToList(),
                TotalQuantity = cartDto.TotalQuantity,
                TotalPrice = cartDto.TotalPrice
            };
        }
    }
}
