using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Common.Interfaces
{
    public interface ICartRepository
    {
        Task<Domain.Entities.Cart?> GetBasketByUsername(string username);
        Task<Domain.Entities.Cart> UpdateBasket(Domain.Entities.Cart basket, DistributedCacheEntryOptions entryOptions);
        Task<bool> DeleteBasketByUsername(string username);
    }
}
