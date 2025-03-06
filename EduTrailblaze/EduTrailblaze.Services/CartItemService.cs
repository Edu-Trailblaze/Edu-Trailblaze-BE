using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.Interfaces;

namespace EduTrailblaze.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IRepository<CartItem, int> _cartItemRepository;

        public CartItemService(IRepository<CartItem, int> cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<CartItem?> GetCartItem(int cartItemId)
        {
            try
            {
                return await _cartItemRepository.GetByIdAsync(cartItemId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the CartItem: " + ex.Message);
            }
        }

        public async Task<IEnumerable<CartItem>> GetCartItems()
        {
            try
            {
                return await _cartItemRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the CartItem: " + ex.Message);
            }
        }

        public async Task AddCartItem(CartItem cartItem)
        {
            try
            {
                await _cartItemRepository.AddAsync(cartItem);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the CartItem: " + ex.Message);
            }
        }

        public async Task UpdateCartItem(CartItem cartItem)
        {
            try
            {
                await _cartItemRepository.UpdateAsync(cartItem);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the CartItem: " + ex.Message);
            }
        }

        public async Task DeleteCartItem(CartItem cartItem)
        {
            try
            {
                await _cartItemRepository.DeleteAsync(cartItem);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the CartItem: " + ex.Message);
            }
        }
    }
}
