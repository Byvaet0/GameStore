using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Endpoints;
using OrderService.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Добавляем подключение к БД
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("OrderDatabase")));




builder.Services.AddHostedService<RabbitMqListener>();


var app = builder.Build();

// Инициализация базы данных (если необходимо)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    DbInitializer.Initialize(context);
}

// (Endpoints)
app.MapOrderEndpoints(); 

app.Run();
