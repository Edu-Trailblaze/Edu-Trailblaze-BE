using EduTrailblaze.Entities;
using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart?> GetCart(int cartId);

        Task<IEnumerable<Cart>> GetCarts();

        Task AddCart(Cart cart);

        Task UpdateCart(Cart cart);

        Task DeleteCart(Cart cart);

        Task DisableCart(int cartId);

        void RemoveCourseFromCookieCart(int courseId, string? userId);

        void DeleteCartInCookie(string? userId);

        int NumberOfItemsInCookieCart(string? userId);

        List<CartItemDTO> GetCookieCart(string? userId);

        Task SaveCartToCookie(int courseId, string? userId);

        Task<Cart> GetSystemCart(string userId);

        Task CreateSystemCart(string userId);

        Task RemoveItemFromSystemCart(int courseId, string userId);

        Task AddItemToSystemCart(int courseId, string userId);

        Task<int> NumberOfItemInSystemCart(string userId);

        Task<List<CartItemDTO>> GetCart(string? userId);

        Task<int> NumberOfItemsInCart(string? userId);

        Task<CartInformation?> ViewCart(string? userId);

        Task AddToCart(string? userId, int courseId);

        Task RemoveFromCart(string? userId, int courseId);

        Task ClearCart(string? userId);

        Task<CartInformation> AddToCartV2(string? userId, int courseId);
    }
}