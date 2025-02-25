using CartService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options)
            : base(options)
        {
        }

        // DbSet для корзины и элементов корзины
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настроим связь между Cart и CartItem
            modelBuilder.Entity<Cart>()
                .HasKey(c => c.CartId);  // CartId как первичный ключ

            modelBuilder.Entity<CartItem>()
                .HasKey(ci => ci.CartItemId);  // CartItemId как первичный ключ

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)  // Указываем, что CartItem связан с Cart
                .WithMany(c => c.Items)  // Cart может содержать несколько CartItem
                .HasForeignKey(ci => ci.CartId);  // CartId — внешний ключ в CartItem

            // Добавление начальных данных для Cart
            modelBuilder.Entity<Cart>().HasData(
                new Cart { CartId = 1, UserId = 1, TotalQuantity = 2, TotalPrice = 89.98m },
                new Cart { CartId = 2, UserId = 2, TotalQuantity = 2, TotalPrice = 26.95m }
            );

            // Добавление начальных данных для CartItem
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem { CartItemId = 1, CartId = 1, GameId = 101, GameName = "Cyberpunk 2077", Price = 49.99m },
                new CartItem { CartItemId = 2, CartId = 1, GameId = 102, GameName = "The Witcher 3", Price = 39.99m },
                new CartItem { CartItemId = 3, CartId = 2, GameId = 103, GameName = "Dota 2", Price = 0.00m },
                new CartItem { CartItemId = 4, CartId = 2, GameId = 104, GameName = "Minecraft", Price = 26.95m }
            );
        }
    }
}
