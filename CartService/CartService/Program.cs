using CartService.Data;
using CartService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CartService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в DI-контейнер
builder.Services.AddControllers();

// Регистрируем CartDbContext
builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрируем RabbitMQ Publisher (для отправки сообщений)
builder.Services.AddSingleton<RabbitMqPublisher>();

// Регистрируем RabbitMQ Listener как фоновую службу
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

// Применяем миграции при старте приложения
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();
    dbContext.Database.Migrate();  // Применяем все миграции
}

// Настроим middleware
app.UseRouting();
app.UseAuthorization();
app.MapCartEndpoints();

app.Run();
