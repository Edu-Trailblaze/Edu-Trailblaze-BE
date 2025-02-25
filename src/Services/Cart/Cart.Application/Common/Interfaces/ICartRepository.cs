using Microsoft.Extensions.Caching.Distributed;

namespace Cart.Application.Common.Interfaces
{
    public interface ICartRepository
    {
        Task<Domain.Entities.Cart?> GetCartByUsername(string username);
        Task<Domain.Entities.Cart> UpdateCart(Domain.Entities.Cart basket, DistributedCacheEntryOptions entryOptions);
        Task<bool> DeleteCartByUsername(string username);
    }
}
