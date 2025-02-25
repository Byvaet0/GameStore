using StackExchange.Redis;

namespace GameStore.Api.Configurations;

public class RedisService
{
    private readonly ConnectionMultiplexer _redis;  //объект для подключения к Redis. Он управляет соединением.
    private readonly IDatabase _db;

    public RedisService(IConfiguration configuration)
    {
        var redisHost = configuration["Redis:Host"];
        var redisPort = configuration["Redis:Port"];

        _redis = ConnectionMultiplexer.Connect($"{redisHost}:{redisPort}");
        _db = _redis.GetDatabase();  //Получаем доступ к базе данных Redis.
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _db.StringSetAsync(key, value, expiry);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }
}
