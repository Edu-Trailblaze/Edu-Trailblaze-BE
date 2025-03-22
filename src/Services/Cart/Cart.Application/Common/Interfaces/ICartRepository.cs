using Cart.Application.Common.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Cart.Application.Common.Interfaces
{
    public interface ICartRepository
    {
        Task<Domain.Entities.Cart?> GetCartByUsername(string username);
        Task<Domain.Entities.Cart> UpdateCart(Domain.Entities.Cart basket, DistributedCacheEntryOptions entryOptions);
        Task<bool> DeleteCartByUsername(string username);
        Task<List<CartItemDTO>> GetCart(string? userId);
       // Task<CartInformation> AddToCartV2(string? userId, int courseId);
        Task<CartInformation?> ViewCart(string? userId);

    }
}
