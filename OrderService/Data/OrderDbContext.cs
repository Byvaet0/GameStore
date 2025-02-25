using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        // Метод для добавления заказа
        public async Task CreateOrderAsync(Order order)
        {
            // Добавляем заказ в базу данных
            await Orders.AddAsync(order);  // Добавляем сам заказ
            await SaveChangesAsync();      // Сохраняем изменения в базе данных
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId); // Устанавливаем внешний ключ

            base.OnModelCreating(modelBuilder);
        }
    }
}
