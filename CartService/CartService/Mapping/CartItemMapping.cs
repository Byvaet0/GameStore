using CartService.Entities;
using CartService.DTOs;

namespace CartService.Mapping
{
    public static class CartItemMapping
    {
        public static CartItemDto ToDto(CartItem cartItem)
        {
            return new CartItemDto
            {
                GameId = cartItem.GameId,
                GameName = cartItem.GameName,
                Price = cartItem.Price
            };
        }

        public static CartItem ToEntity(CartItemDto cartItemDto)
        {
            return new CartItem
            {
                GameId = cartItemDto.GameId,
                GameName = cartItemDto.GameName,
                Price = cartItemDto.Price
            };
        }
    }
}
