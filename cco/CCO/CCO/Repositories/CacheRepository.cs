using CCO.Entities;
using Npgsql;
using StackExchange.Redis;

namespace CCO.Repositories
{
    public class CacheRepository
    {
        public async Task<string> GetAsync(CacheSource cache, string key)
        {
            using var redis = ConnectionMultiplexer.Connect(GetConnectionString(cache));
            IDatabase database = redis.GetDatabase();

            var cachedValue = await database.StringGetAsync(key);

            return cachedValue.ToString();
        }

        public async Task<bool> SetAsync(CacheSource cache, string key, string value, TimeSpan ttl)
        {
            using var redis = ConnectionMultiplexer.Connect(GetConnectionString(cache));
            IDatabase database = redis.GetDatabase();

            return await database.StringSetAsync(key, value, ttl);
        }

        public string GetConnectionString(CacheSource cache)
        {
            return $"{cache.Url},password={cache.Password}";
        }
    }
}
