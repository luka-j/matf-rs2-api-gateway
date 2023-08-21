using CCO.Entities;
using Npgsql;
using StackExchange.Redis;

namespace CCO.Repositories
{
    public class CacheRepository
    {
        public async Task<string> GetAsync(Datasource cache, string key)
        {
            using var redis = ConnectionMultiplexer.Connect(cache.ConnectionString);

            IDatabase database = redis.GetDatabase();

            var cachedValue = await database.StringGetAsync(key);

            return cachedValue.ToString();
        }

        public async Task<bool> SetAsync(Datasource cache, string key, string value, TimeSpan ttl)
        {
            using var redis = ConnectionMultiplexer.Connect(cache.ConnectionString);
            IDatabase database = redis.GetDatabase();

            return await database.StringSetAsync(key, value, ttl);
        }

    }
}
