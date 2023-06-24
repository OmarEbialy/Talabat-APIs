using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services;

namespace Talabat.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        // Post Response in Redis Db
        public async Task CachceResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null) return;

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var cacheResponse = JsonSerializer.Serialize(response , options);

            await _database.StringSetAsync(cacheKey, cacheResponse, timeToLive); // take response as json 
        }

        // Get Cached Response
        public async Task<string?> GetCachedResponse(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;

            //RedisValue obj; // => new Obj From RedisValue Declare Obj From Struct "RedisValue"
        }
    }
}
