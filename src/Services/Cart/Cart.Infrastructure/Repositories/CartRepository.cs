using Cart.Application.Common.Interfaces;
using Contracts.Domain;
using Entity =  Cart.Domain.Entities;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;

        public CartRepository(IDistributedCache distributedCache, ISerializeService serializeService, ILogger logger)
        {
            _redisCacheService = distributedCache;
            _serializeService = serializeService;
            _logger = logger;
        }
        public async Task<bool> DeleteCartByUsername(string username)
        {
            try
            {
                await _redisCacheService.RemoveAsync(username);
                return true;
            }
            catch
            {
                _logger.LogError("Error deleting basket for username: {username}", username);
                return false;
            }
        }

        public async Task<Entity.Cart?> GetCartByUsername(string username)
        {
            var basket = await _redisCacheService.GetStringAsync(username);
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Entity.Cart>(basket);
        }

        public async Task<Entity.Cart> UpdateCart(Entity.Cart basket, DistributedCacheEntryOptions entryOptions)
        {
            if (entryOptions != null)
            {
                await _redisCacheService.SetStringAsync(basket.UserName, _serializeService.Serialize(basket), entryOptions);
            }
            else await _redisCacheService.SetStringAsync(basket.UserName, _serializeService.Serialize(basket));
            return await GetCartByUsername(basket.UserName);
        }
    }
}
