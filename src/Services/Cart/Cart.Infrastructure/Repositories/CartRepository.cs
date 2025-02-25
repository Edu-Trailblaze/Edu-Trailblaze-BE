using Cart.Application.Common.Interfaces;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Entity = Cart.Domain.Entities;
using ILogger = Serilog.ILogger;

namespace Cart.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly IDatabase _database;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;

        public CartRepository(IDistributedCache distributedCache, ISerializeService serializeService, ILogger logger)
        {
            _redisCacheService = distributedCache;
            _serializeService = serializeService;
            _logger = logger;

        }
        public async Task<bool> DeleteCartByUsername(string userName)
        {
            string key = $"Cart:{userName}";
            try
            {
                await _redisCacheService.RemoveAsync(key);
                return true;
            }
            catch
            {
                _logger.Error("Error deleting basket for username: {username}", userName);
                return false;
            }
        }

        public async Task<Entity.Cart?> GetCartByUsername(string userName)
        {
            string key = $"Cart:{userName}";
            var basket = await _redisCacheService.GetStringAsync(key);
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Entity.Cart>(basket);
        }

        public async Task<Entity.Cart> UpdateCart(Entity.Cart basket, DistributedCacheEntryOptions entryOptions)
        {
            string key = $"Cart:{basket.UserName}";
            if (entryOptions != null)
            {
                await _redisCacheService.SetStringAsync(key, _serializeService.Serialize(basket), entryOptions);
            }
            else await _redisCacheService.SetStringAsync(key, _serializeService.Serialize(basket));
            return await GetCartByUsername(basket.UserName);
        }
    }
}
