using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

// Добавляем поддержку CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin() // Разрешаем все источники (вы можете ограничить доступ только определенными доменами)
               .AllowAnyMethod()  // Разрешаем любые HTTP методы (GET, POST, PUT, DELETE)
               .AllowAnyHeader(); // Разрешаем любые заголовки
    });
});

var app = builder.Build();

// Применяем CORS политику
app.UseCors("AllowAllOrigins");


app.MapGamesEndpoints();
app.MapGenresEndpoints();

app.MigrateDbAsync();  //автоматически создаст БД, если ее не будет







app.Run();
