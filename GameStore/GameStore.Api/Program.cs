using System.Text;
using Microsoft.IdentityModel.Tokens;
using GameStore.Api.Configurations;
using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GameStore.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Получаем строку подключения к базе данных
var connString = builder.Configuration.GetConnectionString("GameStore");

// Регистрация контекста базы данных
builder.Services.AddSqlite<GameStoreContext>(connString);

// Регистрация сервисов
builder.Services.AddControllers();
builder.Services.AddSingleton<RedisService>();

// Добавление поддержки CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin() // Разрешаем все источники
               .AllowAnyMethod()  // Разрешаем любые HTTP методы
               .AllowAnyHeader(); // Разрешаем любые заголовки
    });
});

// Загружаем настройки для JWT
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<RabbitMqService>();


// Регистрируем AuthService
builder.Services.AddSingleton<AuthService>();

// Настройка аутентификации через JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

// Добавляем авторизацию
builder.Services.AddAuthorization();

var app = builder.Build();

// Применяем CORS политику
app.UseCors("AllowAllOrigins");

// Включаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

// Применяем миграции базы данных
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
    dbContext.Database.Migrate(); // Применение миграции синхронно
}

// Регистрируем маршруты для эндпоинтов
app.MapGamesEndpoints();
app.MapGenresEndpoints();
app.MapUsersEndpoints();

// Регистрируем контроллеры
app.MapControllers();

// Запускаем приложение
app.Run();
