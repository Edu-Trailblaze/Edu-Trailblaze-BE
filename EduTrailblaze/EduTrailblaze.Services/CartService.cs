using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Helper;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduTrailblaze.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart, int> _cartRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICourseService _courseService;
        private readonly IReviewService _reviewService;

        public CartService(IRepository<Cart, int> cartRepository, IHttpContextAccessor httpContextAccessor, ICourseService courseService, UserManager<User> userManager, IReviewService reviewService)
        {
            _cartRepository = cartRepository;
            _httpContextAccessor = httpContextAccessor;
            _courseService = courseService;
            _userManager = userManager;
            _reviewService = reviewService;
        }

        public async Task<Cart?> GetCart(int cartId)
        {
            try
            {
                return await _cartRepository.GetByIdAsync(cartId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the Cart: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Cart>> GetCarts()
        {
            try
            {
                return await _cartRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the Cart: " + ex.Message);
            }
        }

        public async Task AddCart(Cart cart)
        {
            try
            {
                await _cartRepository.AddAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the Cart: " + ex.Message);
            }
        }

        public async Task UpdateCart(Cart cart)
        {
            try
            {
                await _cartRepository.UpdateAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the Cart: " + ex.Message);
            }
        }

        public async Task DisableCart(int cartId)
        {
            try
            {
                var cart = await _cartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    throw new Exception("Cart not found.");
                }

                cart.IsActive = false;
                await _cartRepository.UpdateAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the Cart: " + ex.Message);
            }
        }

        public async Task DeleteCart(Cart cart)
        {
            try
            {
                await _cartRepository.DeleteAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the Cart: " + ex.Message);
            }
        }

        // Cookie cart
        public void RemoveCourseFromCookieCart(int courseId, string? userId)
        {
            try
            {
                Dictionary<int, CartItemDTO> cartItems = new Dictionary<int, CartItemDTO>();
                var savedCart = _httpContextAccessor.HttpContext?.Request.Cookies[$"Cart_{userId}"] ?? string.Empty;

                if (!string.IsNullOrEmpty(savedCart))
                {
                    cartItems = CartUtil.GetCartFromCookie(savedCart);
                    cartItems.Remove(courseId);
                }

                var strItemsInCart = CartUtil.ConvertCartToString(cartItems.Values.ToList());
                CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing the item from the cart: " + ex.Message);
            }
        }

        public void DeleteCartInCookie(string? userId)
        {
            try
            {
                CartUtil.DeleteCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the cart: " + ex.Message);
            }
        }

        public int NumberOfItemsInCookieCart(string? userId)
        {
            try
            {
                int count = 0;
                string savedCart;
                if (string.IsNullOrEmpty(userId))
                {
                    savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"Cart"];
                }
                else
                {
                    savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"Cart_{userId}"];
                }
                if (!string.IsNullOrEmpty(savedCart))
                {
                    var cartItems = CartUtil.GetCartFromCookie(savedCart);
                    // Directly sum up the quantities from cartItems
                    count = cartItems.Count;
                }
                return count;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting number of items in cart: {ex.Message}");
                return 0;
            }
        }

        public List<CartItemDTO> GetCookieCart(string? userId)
        {
            try
            {
                string savedCart = _httpContextAccessor.HttpContext.Request.Cookies[$"Cart_{userId}"];

                if (!string.IsNullOrEmpty(savedCart))
                {
                    var cart = CartUtil.GetCartFromCookie(savedCart);
                    var cartItems = cart.Values.ToList();
                    return cartItems;
                }
                return new List<CartItemDTO>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting cart: {ex.Message}");
                return new List<CartItemDTO>();
            }
        }

        public async Task SaveCartToCookie(int courseId, string? userId)
        {
            try
            {
                Dictionary<int, CartItemDTO> cartItems = new Dictionary<int, CartItemDTO>();
                CartItemDTO item = null;

                var savedCart = _httpContextAccessor.HttpContext?.Request.Cookies[$"Cart_{userId}"] ?? string.Empty;

                if (!string.IsNullOrEmpty(savedCart))
                {
                    cartItems = CartUtil.GetCartFromCookie(savedCart);
                }

                // Check if the item exists in the cart, add or update accordingly
                if (cartItems.TryGetValue(courseId, out item))
                {
                    //throw new Exception("Course already exists in the cart.");
                    return;
                }

                item = new CartItemDTO
                {
                    ItemId = courseId,
                };
                cartItems[courseId] = item;

                // Convert the updated cart to string and save it back to the cookie
                var strItemsInCart = CartUtil.ConvertCartToString(cartItems.Values.ToList());
                CartUtil.SaveCartToCookie(_httpContextAccessor.HttpContext.Request, _httpContextAccessor.HttpContext.Response, strItemsInCart, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the cart to cookie: " + ex.Message);
            }
        }

        // System cart
        public async Task<Cart> GetSystemCart(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("Invalid user id");
                }

                var cartDbset = await _cartRepository.GetDbSet();
                var cart = await cartDbset
                    .Include(c => c.CartItems)
                    .Where(c => c.UserId == userId && c.IsActive)
                    .OrderByDescending(c => c.UpdatedAt)
                    .FirstOrDefaultAsync();

                if (cart != null)
                {
                    return cart;
                }

                await CreateSystemCart(userId);

                return await GetSystemCart(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the Cart: " + ex.Message);
            }
        }

        public async Task CreateSystemCart(string userId)
        {
            try
            {
                var cart = new Cart
                {
                    UserId = userId,
                };
                await _cartRepository.AddAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the Cart: " + ex.Message);
            }
        }

        public async Task RemoveItemFromSystemCart(int courseId, string userId)
        {
            try
            {
                var cart = await GetSystemCart(userId);
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CourseId == courseId);
                if (cartItem == null)
                {
                    throw new Exception("Course not found in the cart.");
                }
                cart.CartItems.Remove(cartItem);
                await _cartRepository.UpdateAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing the course from the cart: " + ex.Message);
            }
        }

        public async Task AddItemToSystemCart(int courseId, string userId)
        {
            try
            {
                var cart = await GetSystemCart(userId);
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CourseId == courseId);
                if (cartItem != null)
                {
                    throw new Exception("Course already exists in the cart.");
                }
                cart.CartItems.Add(new CartItem
                {
                    CourseId = courseId,
                    CartId = cart.Id
                });
                await _cartRepository.UpdateAsync(cart);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the course to the cart: " + ex.Message);
            }
        }

        public async Task<int> NumberOfItemInSystemCart(string userId)
        {
            try
            {
                var cart = await GetSystemCart(userId);
                return cart.CartItems.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of items in the cart: " + ex.Message);
            }
        }

        // Combined cart

        public async Task<List<CartItemDTO>> GetCart(string? userId)
        {
            try
            {
                var cookieCart = GetCookieCart(userId);
                var cartItems = new List<CartItemDTO>();

                if (cookieCart != null && cookieCart.Count > 0)
                {
                    cartItems.AddRange(cookieCart);
                }

                if (userId != null)
                {
                    var systemCart = await GetSystemCart(userId);
                    var systemCartItems = systemCart.CartItems.Select(ci => new CartItemDTO
                    {
                        ItemId = ci.CourseId,
                    }).ToList();

                    // Merge system cart items with cookie cart items, avoiding duplicates
                    foreach (var item in systemCartItems)
                    {
                        if (!cartItems.Any(ci => ci.ItemId == item.ItemId))
                        {
                            cartItems.Add(item);
                        }
                    }
                }

                foreach (var item in cartItems)
                {
                    await SaveCartToCookie(item.ItemId, userId);
                }

                return cartItems;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while viewing the cart: " + ex.Message);
            }
        }

        public async Task<CartInformation?> ViewCart(string? userId)
        {
            try
            {
                var cartItems = await GetCart(userId);

                if (cartItems.Count == 0)
                {
                    return null;
                }

                var cartInformation = new CartInformation
                {
                    CartItems = new List<CartItemInformation>()
                };
                decimal totalPrice = 0;

                foreach (var item in cartItems)
                {
                    var course = await _courseService.GetCartCourseInformationAsync(item.ItemId);
                    var coursePrice = course.Price;

                    var instructors = await _courseService.InstructorInformation(item.ItemId);

                    var discount = await _courseService.DiscountInformationResponse(item.ItemId);
                    if (discount != null)
                    {
                        discount.CalculateDiscountAndPrice(coursePrice);
                        coursePrice = discount.CalculatedPrice;
                    }

                    var coupon = await _courseService.CouponInformation(item.ItemId, userId);
                    if (coupon != null)
                    {
                        coupon.CalculateDiscountAndPrice(coursePrice);
                        coursePrice = coupon.CalculatedPrice;
                    }

                    totalPrice += coursePrice;

                    cartInformation.CartItems.Add(
                        new CartItemInformation
                        {
                            CartCourseInformation = course,
                            InstructorInformation = instructors,
                            CouponInformation = coupon,
                            DiscountInformation = discount,
                            CourseReviewInformation = await _reviewService.GetAverageRatingAndNumberOfRatings(item.ItemId),
                            TotalCoursePrice = coursePrice
                        }
                    );
                }
                cartInformation.TotalPrice = totalPrice;
                return cartInformation;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while viewing the cart: " + ex.Message);
            }
        }

        public async Task<int> NumberOfItemsInCart(string? userId)
        {
            try
            {
                var num = NumberOfItemsInCookieCart(userId);

                if (userId != null && num == 0)
                {
                    num = await NumberOfItemInSystemCart(userId);
                }

                return num;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the number of items in the cart: " + ex.Message);
            }
        }

        public async Task AddToCart(string? userId, int courseId)
        {
            try
            {
                var cart = await GetCart(userId);

                if (cart.Any(ci => ci.ItemId == courseId))
                {
                    throw new Exception("Course already exists in the cart.");
                }

                await SaveCartToCookie(courseId, userId);
                if (userId != null)
                {
                    await AddItemToSystemCart(courseId, userId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the course to the cart: " + ex.Message);
            }
        }

        public async Task<CartInformation> AddToCartV2(string? userId, int courseId)
        {
            try
            {
                var cart = await GetCart(userId);

                if (cart.Any(ci => ci.ItemId == courseId))
                {
                    throw new Exception("Course already exists in the cart.");
                }

                await SaveCartToCookie(courseId, userId);
                if (userId != null)
                {
                    await AddItemToSystemCart(courseId, userId);
                }

                return await ViewCart(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the course to the cart: " + ex.Message);
            }
        }

        public async Task RemoveFromCart(string? userId, int courseId)
        {
            try
            {
                var cart = await GetCart(userId);

                if (!cart.Any(ci => ci.ItemId == courseId))
                {
                    throw new Exception("Course not found in the cart.");
                }

                RemoveCourseFromCookieCart(courseId, userId);

                if (userId != null)
                {
                    await RemoveItemFromSystemCart(courseId, userId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing the course from the cart: " + ex.Message);
            }
        }

        public async Task ClearCart(string? userId)
        {
            try
            {
                var cart = await GetCart(userId);

                if (cart.Count == 0)
                {
                    return;
                }
                DeleteCartInCookie(userId);
                if (userId != null)
                {
                    var systemCart = await GetSystemCart(userId);
                    await DeleteCart(systemCart);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while clearing the cart: " + ex.Message);
            }
        }
    }
}
